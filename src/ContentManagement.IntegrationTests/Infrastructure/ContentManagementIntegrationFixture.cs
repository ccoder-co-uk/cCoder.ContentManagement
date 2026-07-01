using cCoder.Data;
using cCoder.Data.Models.Security;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        string coreConnectionString = AddDatabaseSuffix("CCODER_ACCEPTANCE_CORE_CONNECTION_STRING");
        string ssoConnectionString = AddDatabaseSuffix("CCODER_ACCEPTANCE_SSO_CONNECTION_STRING");

        databaseServices = CreateDatabaseServices(coreConnectionString, ssoConnectionString);
        await ResetDatabasesAsync();
        await SeedGuestUserAsync();

        Factory = new ContentManagementIntegrationFactory(
            coreConnectionString,
            ssoConnectionString,
            DecryptionKey);

        Client = Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost"),
        });
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();

        if (Factory is not null)
            await Factory.DisposeAsync();

        await DropDatabasesAsync();

        if (databaseServices is not null)
            await databaseServices.DisposeAsync();
    }

    private async Task ResetDatabasesAsync()
    {
        using IServiceScope scope = databaseServices.CreateScope();
        using var sso = scope.ServiceProvider.GetRequiredService<ISecurityDbContextFactory>()
            .CreateDbContext(true);
        using var core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        EnsureSafeIntegrationDatabase(sso.Database.GetConnectionString(), "dev-Members");
        EnsureSafeIntegrationDatabase(core.Database.GetConnectionString(), "dev-Core");

        ForceDropDatabase(sso.Database.GetConnectionString());
        ForceDropDatabase(core.Database.GetConnectionString());

        await sso.Database.MigrateAsync();
        await core.Database.MigrateAsync();
    }

    private async Task DropDatabasesAsync()
    {
        if (databaseServices is null)
            return;

        using IServiceScope scope = databaseServices.CreateScope();
        using var sso = scope.ServiceProvider.GetRequiredService<ISecurityDbContextFactory>()
            .CreateDbContext(true);
        using var core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        EnsureSafeIntegrationDatabase(sso.Database.GetConnectionString(), "dev-Members");
        EnsureSafeIntegrationDatabase(core.Database.GetConnectionString(), "dev-Core");

        ForceDropDatabase(sso.Database.GetConnectionString());
        ForceDropDatabase(core.Database.GetConnectionString());
    }

    private async Task SeedGuestUserAsync()
    {
        using IServiceScope scope = databaseServices.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        if (!await core.Set<User>().AnyAsync(user => user.Id == "Guest"))
        {
            await core.Set<User>().AddAsync(new User
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
        services.AddSingleton(
            new Config
            {
                ConnectionStrings = new Dictionary<string, string>
                {
                    ["Core"] = coreConnectionString,
                    ["SSO"] = ssoConnectionString,
                },
                Settings = new Dictionary<string, string>
                {
                    ["DecryptionKey"] = DecryptionKey,
                    ["enableExternalEventing"] = "true",
                },
                Services = new Dictionary<string, string>(),
            });
        services.AddSingleton<ISecurityDbContextFactory>(
            _ => new MSSQLSecurityDbContextFactory(ssoConnectionString));
        services.AddCoreData(coreConnectionString);

        return services.BuildServiceProvider(validateScopes: false);
    }

    private static void EnsureSafeIntegrationDatabase(string connectionString, string protectedDatabaseName)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Integration database connection string is empty.");

        SqlConnectionStringBuilder builder = CreateConnectionStringBuilder(connectionString);
        string databaseName = builder.InitialCatalog ?? string.Empty;

        if (string.IsNullOrWhiteSpace(databaseName))
            throw new InvalidOperationException("Integration database name is empty.");

        if (databaseName.Equals(protectedDatabaseName, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                $"Refusing to run integration database operations against protected database '{protectedDatabaseName}'.");

        if (!databaseName.Contains("integration", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                $"Refusing to run integration database operations against non-integration database '{databaseName}'.");
    }

    private static void ForceDropDatabase(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return;

        SqlConnectionStringBuilder builder = CreateConnectionStringBuilder(connectionString);
        string databaseName = builder.InitialCatalog ?? string.Empty;

        if (string.IsNullOrWhiteSpace(databaseName))
            return;

        builder.InitialCatalog = "master";

        using SqlConnection connection = new(builder.ConnectionString);
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
        _ = command.Parameters.AddWithValue("@databaseName", databaseName);
        command.ExecuteNonQuery();
    }

    private static string AddDatabaseSuffix(string variableName)
    {
        string connectionString =
            Environment.GetEnvironmentVariable(variableName)
            ?? Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.User)
            ?? Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Machine)
            ?? ReadConfiguredConnectionString(variableName);

        if (string.IsNullOrWhiteSpace(connectionString))
            return string.Empty;

        SqlConnectionStringBuilder builder = CreateConnectionStringBuilder(connectionString);

        if (!string.IsNullOrWhiteSpace(builder.InitialCatalog))
            builder.InitialCatalog = $"{builder.InitialCatalog}-contentmanagement-integration";

        return builder.ConnectionString;
    }

    private static string ReadConfiguredConnectionString(string variableName)
    {
        string connectionName = variableName.Contains("CORE", StringComparison.OrdinalIgnoreCase)
            ? "Core"
            : "SSO";

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.testing.json", optional: true)
            .Build();

        return configuration.GetConnectionString(connectionName) ?? string.Empty;
    }

    private static SqlConnectionStringBuilder CreateConnectionStringBuilder(string connectionString) =>
        new(connectionString)
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
