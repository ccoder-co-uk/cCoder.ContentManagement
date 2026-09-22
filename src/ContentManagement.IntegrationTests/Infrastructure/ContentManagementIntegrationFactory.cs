// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Models;
using ContentManagement.Web;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

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
                new KeyValuePair<string, string>(key: "CoreData:ConnectionString", value: coreConnectionString),
                new KeyValuePair<string, string>(key: "SecurityData:ConnectionString", value: ssoConnectionString),
                new KeyValuePair<string, string>(key: "Security:DecryptionKey", value: decryptionKey),
                new KeyValuePair<string, string>(key: "Eventing:ProviderType", value: "Http"),
                new KeyValuePair<string, string>(key: "Eventing:Http:MaxConcurrency", value: "1"),
            ]);
        });

        builder.ConfigureTestServices(servicesConfiguration: services =>
        {
            services.RemoveAll<ILoggerProvider>();

            services.AddDataProtection()
                .UseEphemeralDataProtectionProvider();

            services.RemoveAll<ICoreContextFactory>();
            services.RemoveAll<ISecurityDbContextFactory>();
            services.RemoveAll<cCoder.Data.Models.DataConfiguration>();

            services.AddData(
                configuration: new cCoder.Data.Models.DataConfiguration
                {
                    ConnectionString = coreConnectionString,
                });

            services.AddSecurityData(
                configuration: new SecurityDataConfiguration
                {
                    ConnectionString = ssoConnectionString
                });

        });
    }
}