// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Dependencies;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects;
using ContentManagement.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ContentManagement.IntegrationTests.Infrastructure;

internal sealed class ContentManagementIntegrationFactory(
        string coreConnectionString,
        string ssoConnectionString,
        string decryptionKey)
            : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(environment: "Acceptance");

        builder.ConfigureAppConfiguration(configureDelegate: (_, config) =>
        {
            config.AddInMemoryCollection(
initialData: [
                new KeyValuePair<string, string>(key: "ConnectionStrings:Core", value: coreConnectionString),
                new KeyValuePair<string, string>(key: "ConnectionStrings:SSO", value: ssoConnectionString),
                new KeyValuePair<string, string>(key: "Settings:DecryptionKey", value: decryptionKey),
                new KeyValuePair<string, string>(key: "Settings:enableExternalEventing", value: "true"),
                new KeyValuePair<string, string>(key: "Eventing:ProviderType", value: "Http"),
                new KeyValuePair<string, string>(key: "Eventing:Http:MaxConcurrency", value: "1"),
            ]);
        });

        builder.ConfigureTestServices(servicesConfiguration: services =>
        {
            services.RemoveAll<ICoreContextFactory>();
            services.RemoveAll<ISecurityDbContextFactory>();

            services.AddSingleton(
implementationInstance: new Config
{
    ConnectionStrings = new Dictionary<string, string>
    {
        ["Core"] = coreConnectionString,
        ["SSO"] = ssoConnectionString,
    },
    Settings = new Dictionary<string, string>
    {
        ["DecryptionKey"] = decryptionKey,
        ["enableExternalEventing"] = "true",
    },
    Services = new Dictionary<string, string>(),
});

            services.AddSingleton<ISecurityDbContextFactory>(
implementationFactory: _ => new MSSQLSecurityDbContextFactory(connectionString: ssoConnectionString));

            services.AddCoreData(connectionString: coreConnectionString);
        });
    }
}