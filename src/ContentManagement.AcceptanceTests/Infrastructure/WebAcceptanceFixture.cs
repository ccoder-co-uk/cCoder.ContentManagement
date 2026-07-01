using cCoder.ContentManagement;
using cCoder.Data;
using cCoder.Security.Data.EF;
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
            CoreConnectionString = AddDatabaseSuffix("CCODER_ACCEPTANCE_CORE_CONNECTION_STRING"),
            SsoConnectionString = AddDatabaseSuffix("CCODER_ACCEPTANCE_SSO_CONNECTION_STRING"),
            DecryptionKey = "000000000000000000000000000000000000000000000000",
        };

        databaseServices = CreateDatabaseServices(settings);
        databaseManager = new AcceptanceDatabaseManager(databaseServices);
        await databaseManager.ResetDatabasesAsync();
        await SeedAsync(databaseServices);

        Factory = new WebAcceptanceFactory(settings);
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

        if (databaseManager is not null)
            await databaseManager.DropDatabasesAsync();

        if (databaseServices is not null)
            await databaseServices.DisposeAsync();
    }

    private static Task SeedAsync(IServiceProvider services) =>
        new AcceptanceApplicationSeeder(services).SeedAsync();

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
        services.AddSingleton(dataConfig);
        services.AddSingleton(
            new cCoder.ContentManagement.Models.Config
            {
                ConnectionStrings = new Dictionary<string, string>(dataConfig.ConnectionStrings),
                Settings = new Dictionary<string, string>(dataConfig.Settings),
                Services = new Dictionary<string, string>(dataConfig.Services),
            });
        services.AddSingleton<ISecurityDbContextFactory>(
            _ => new MSSQLSecurityDbContextFactory(settings.SsoConnectionString));
        services.AddCoreData(settings.CoreConnectionString);
        services.AddContentManagementHostedServices();

        return services.BuildServiceProvider(validateScopes: false);
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

        SqlConnectionStringBuilder builder = new(connectionString)
        {
            Encrypt = true,
            TrustServerCertificate = true,
        };
        string databaseName = builder.InitialCatalog ?? string.Empty;

        if (string.IsNullOrWhiteSpace(databaseName))
            return connectionString;

        string suffix = typeof(WebAcceptanceFixture).Assembly.GetName().Name!
            .Replace(".AcceptanceTests", string.Empty, StringComparison.Ordinal)
            .ToLowerInvariant();

        builder.InitialCatalog = $"{databaseName}-{suffix}";
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
}

[CollectionDefinition(Name)]
public sealed class WebAcceptanceCollection : ICollectionFixture<WebAcceptanceFixture>
{
    public const string Name = "Web acceptance";
}


