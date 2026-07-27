// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement;
using cCoder.Data;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Dependencies;
using cCoder.Security.Data.EF.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web.AcceptanceTests.Models;
using Xunit;


namespace Web.AcceptanceTests.Infrastructure;

public sealed class WebAcceptanceFixture : IAsyncLifetime
{
    private AcceptanceDatabaseManager databaseManager;
    private ServiceProvider databaseServices;

    internal WebAcceptanceFactory Factory { get; private set; } = null!;

    public HttpClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        AcceptanceSettings settings = new()
        {
            CoreConnectionString = AddDatabaseSuffix(variableName: "ConnectionStrings__Core"),
            SsoConnectionString = AddDatabaseSuffix(variableName: "ConnectionStrings__SSO"),
            DecryptionKey = "000000000000000000000000000000000000000000000000",
        };

        databaseServices = CreateDatabaseServices(settings: settings);
        databaseManager = new AcceptanceDatabaseManager(services: databaseServices);
        await databaseManager.ResetDatabasesAsync();
        await SeedAsync(services: databaseServices);

        Factory = new WebAcceptanceFactory(settings: settings);

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

        if (databaseManager is not null)
        {
            await databaseManager.DropDatabasesAsync();
        }

        if (databaseServices is not null)
        {
            await databaseServices.DisposeAsync();
        }
    }

    private static Task SeedAsync(IServiceProvider services) =>
        new AcceptanceApplicationSeeder(services: services).SeedAsync();

    private static ServiceProvider CreateDatabaseServices(AcceptanceSettings settings)
    {
        ServiceCollection services = new();

        cCoder.Data.Config dataConfig = new()
        {
            ConnectionStrings = new Dictionary<string, string>
            {
                ["Core"] = settings.CoreConnectionString,
                ["SSO"] = settings.SsoConnectionString,
            },
            Settings = new Dictionary<string, string>
            {
                ["DecryptionKey"] = settings.DecryptionKey,
                ["enableExternalEventing"] = "false",
            },
            Services = new Dictionary<string, string>(),
        };

        services.AddLogging();
        services.AddSingleton(implementationInstance: dataConfig);

        services.AddSingleton(
implementationInstance: new cCoder.ContentManagement.Models.Config
{
    ConnectionStrings = new Dictionary<string, string>(dictionary: dataConfig.ConnectionStrings),
    Settings = new Dictionary<string, string>(dictionary: dataConfig.Settings),
    Services = new Dictionary<string, string>(dictionary: dataConfig.Services),
});

        services.AddSingleton<ISecurityDbContextFactory>(
implementationFactory: _ => new MSSQLSecurityDbContextFactory(connectionString: settings.SsoConnectionString));

        services.AddCoreData(connectionString: settings.CoreConnectionString);
        services.AddContentManagementHostedServices();

        return services.BuildServiceProvider(validateScopes: false);
    }

    private static string AddDatabaseSuffix(string variableName)
    {
        string connectionString =
            Environment.GetEnvironmentVariable(variable: variableName)
            ?? Environment.GetEnvironmentVariable(variable: variableName, target: EnvironmentVariableTarget.User)
            ?? Environment.GetEnvironmentVariable(variable: variableName, target: EnvironmentVariableTarget.Machine)
            ?? ReadConfiguredConnectionString(variableName: variableName);

        if (string.IsNullOrWhiteSpace(value: connectionString))
        {
            return string.Empty;
        }

        SqlConnectionStringBuilder builder = new(connectionString: connectionString)
        {
            Encrypt = true,
            TrustServerCertificate = true,
        };

        string databaseName = builder.InitialCatalog ?? string.Empty;

        if (string.IsNullOrWhiteSpace(value: databaseName))
        {
            return connectionString;
        }

        builder.InitialCatalog = $"{databaseName}-acceptance-{Guid.NewGuid():N}";
        return builder.ConnectionString;
    }

    private static string ReadConfiguredConnectionString(string variableName)
    {
        string connectionName = variableName.Contains(value: "CORE", comparisonType: StringComparison.OrdinalIgnoreCase)
            ? "Core"
            : "SSO";

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(basePath: AppContext.BaseDirectory)
            .AddJsonFile(path: "appsettings.testing.json", optional: true)
            .Build();

        return configuration.GetConnectionString(name: connectionName) ?? string.Empty;
    }
}

[CollectionDefinition(Name)]
public sealed class WebAcceptanceCollection : ICollectionFixture<WebAcceptanceFixture>
{
    public const string Name = "Web acceptance";
}