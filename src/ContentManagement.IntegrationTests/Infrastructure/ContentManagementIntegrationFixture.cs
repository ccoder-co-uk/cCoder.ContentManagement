// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Security;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Dependencies;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ContentManagement.IntegrationTests.Infrastructure;

public sealed class ContentManagementIntegrationFixture : IAsyncLifetime
{
    private const string DecryptionKey = "000000000000000000000000000000000000000000000000";
    private ServiceProvider databaseServices;

    internal ContentManagementIntegrationFactory Factory { get; private set; }

    public HttpClient Client { get; private set; }

    public async Task InitializeAsync()
    {
        IntegrationConfiguration configuration =
            IntegrationConfiguration.Create();

        string coreConnectionString = configuration.CoreConnectionString;
        string ssoConnectionString = configuration.SsoConnectionString;

        databaseServices = CreateDatabaseServices(coreConnectionString: coreConnectionString, ssoConnectionString: ssoConnectionString);
        await ResetDatabasesAsync();
        await SeedGuestUserAsync();

        Factory = new ContentManagementIntegrationFactory(
coreConnectionString: coreConnectionString,
ssoConnectionString: ssoConnectionString,
decryptionKey: DecryptionKey);

        Client = Factory.CreateClient(options: new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri(uriString: "https://localhost"),
        });
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();

        if (Factory is not null)
        {
            await Factory.DisposeAsync();
        }

        await DropDatabasesAsync();

        if (databaseServices is not null)
        {
            await databaseServices.DisposeAsync();
        }
    }

    private async Task ResetDatabasesAsync()
    {
        using IServiceScope scope = databaseServices.CreateScope();

        using var sso = scope.ServiceProvider.GetRequiredService<ISecurityDbContextFactory>()
            .CreateDbContext(ignoreAuthInfo: true);

        using var core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        EnsureSafeIntegrationDatabase(connectionString: sso.Database.GetConnectionString(), protectedDatabaseName: "dev-Members");
        EnsureSafeIntegrationDatabase(connectionString: core.Database.GetConnectionString(), protectedDatabaseName: "dev-Core");

        ForceDropDatabase(connectionString: sso.Database.GetConnectionString());
        ForceDropDatabase(connectionString: core.Database.GetConnectionString());

        await sso.Database.MigrateAsync();
        await core.Database.MigrateAsync();
    }

    private async Task DropDatabasesAsync()
    {
        if (databaseServices is null)
        {
            return;
        }

        using IServiceScope scope = databaseServices.CreateScope();

        using var sso = scope.ServiceProvider.GetRequiredService<ISecurityDbContextFactory>()
            .CreateDbContext(ignoreAuthInfo: true);

        using var core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        EnsureSafeIntegrationDatabase(connectionString: sso.Database.GetConnectionString(), protectedDatabaseName: "dev-Members");
        EnsureSafeIntegrationDatabase(connectionString: core.Database.GetConnectionString(), protectedDatabaseName: "dev-Core");

        ForceDropDatabase(connectionString: sso.Database.GetConnectionString());
        ForceDropDatabase(connectionString: core.Database.GetConnectionString());
    }

    private async Task SeedGuestUserAsync()
    {
        using IServiceScope scope = databaseServices.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        if (!await core.Set<User>()
            .AnyAsync(predicate: user => user.Id == "Guest"))
        {
            await core.Set<User>()
                .AddAsync(entity: new User
                {
                    Id = "Guest",
                    DisplayName = "Guest",
                    Email = string.Empty,
                    DefaultCultureId = string.Empty,
                    IsActive = true,
                });

            await core.SaveChangesAsync();
        }
    }

    private static ServiceProvider CreateDatabaseServices(
        string coreConnectionString,
        string ssoConnectionString)
    {
        ServiceCollection services = new();
        services.AddLogging();

        cCoder.Data.Models.DataConfiguration dataConfiguration = new()
        {
            ConnectionString = coreConnectionString,
        };

        services.AddSingleton(
            implementationInstance: dataConfiguration);

        services.AddSingleton<ISecurityDbContextFactory>(
implementationFactory: _ => new MSSQLSecurityDbContextFactory(connectionString: ssoConnectionString));

        services.AddData(configuration: dataConfiguration);

        return services.BuildServiceProvider(validateScopes: false);
    }

    private static void EnsureSafeIntegrationDatabase(string connectionString, string protectedDatabaseName)
    {
        if (string.IsNullOrWhiteSpace(value: connectionString))
        {
            throw new InvalidOperationException(message: "Integration database connection string is empty.");
        }

        SqlConnectionStringBuilder builder = CreateConnectionStringBuilder(connectionString: connectionString);
        string databaseName = builder.InitialCatalog ?? string.Empty;

        if (string.IsNullOrWhiteSpace(value: databaseName))
        {
            throw new InvalidOperationException(message: "Integration database name is empty.");
        }

        if (databaseName.Equals(value: protectedDatabaseName, comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
        message: $"Refusing to run integration database operations against protected database '{protectedDatabaseName}'.");
        }

        if (!databaseName.Contains(value: "-acceptance-", comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
        message: $"Refusing to run integration database operations against non-acceptance database '{databaseName}'.");
        }
    }

    private static void ForceDropDatabase(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(value: connectionString))
        {
            return;
        }

        SqlConnectionStringBuilder builder = CreateConnectionStringBuilder(connectionString: connectionString);
        string databaseName = builder.InitialCatalog ?? string.Empty;

        if (string.IsNullOrWhiteSpace(value: databaseName))
        {
            return;
        }

        builder.InitialCatalog = "master";

        using SqlConnection connection = new(connectionString: builder.ConnectionString);
        connection.Open();

        using SqlCommand command = connection.CreateCommand();

        command.CommandText = @"
IF DB_ID(@databaseName) IS NOT NULL
BEGIN
    DECLARE @sql nvarchar(max) =
        N'ALTER DATABASE [' + REPLACE(@databaseName, ']', ']]') + N'] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;'
        + N'DROP DATABASE [' + REPLACE(@databaseName, ']', ']]') + N']';
    EXEC(@sql);
END";

        _ = command.Parameters.AddWithValue(parameterName: "@databaseName", value: databaseName);
        command.ExecuteNonQuery();
    }

    private static SqlConnectionStringBuilder CreateConnectionStringBuilder(string connectionString) =>
        new(connectionString: connectionString)
        {
            Encrypt = true,
            TrustServerCertificate = true,
        };
}

[CollectionDefinition(Name)]
public sealed class ContentManagementIntegrationCollection
    : ICollectionFixture<ContentManagementIntegrationFixture>
{
    public const string Name = "ContentManagement integration";
}