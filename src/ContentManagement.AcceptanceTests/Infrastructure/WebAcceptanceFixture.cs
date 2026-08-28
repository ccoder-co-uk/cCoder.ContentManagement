// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement;
using cCoder.Data;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Dependencies;
using cCoder.Security.Data.EF.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
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
        AcceptanceSettings settings =
            AcceptanceConfiguration.CreateSettings();

        databaseServices = CreateDatabaseServices(settings: settings);
        databaseManager = new AcceptanceDatabaseManager(services: databaseServices);
        await databaseManager.ResetDatabasesAsync();

        Factory = new WebAcceptanceFactory(settings: settings);
        await SeedAsync(services: Factory.Services);

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

        cCoder.Data.Models.DataConfiguration dataConfig = new()
        {
            ConnectionString = settings.CoreConnectionString,
        };

        services.AddLogging();
        services.AddSingleton(implementationInstance: dataConfig);

        services.AddSingleton<ISecurityDbContextFactory>(
            implementationFactory: _ =>
                new MSSQLSecurityDbContextFactory(
                    connectionString: settings.SsoConnectionString));

        services.AddData(configuration: dataConfig);

        services.AddContentManagementHostedServices(
            configuration:
                new cCoder.ContentManagement.Models
                    .ContentManagementConfiguration());

        return services.BuildServiceProvider(validateScopes: false);
    }
}