using cCoder.Data;
using cCoder.Security.Data.EF;
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
        builder.UseEnvironment("Acceptance");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(
            [
                new KeyValuePair<string, string>("ConnectionStrings:Core", coreConnectionString),
                new KeyValuePair<string, string>("ConnectionStrings:SSO", ssoConnectionString),
                new KeyValuePair<string, string>("Settings:DecryptionKey", decryptionKey),
                new KeyValuePair<string, string>("Settings:enableExternalEventing", "true"),
                new KeyValuePair<string, string>("Eventing:ProviderType", "Http"),
                new KeyValuePair<string, string>("Eventing:Http:MaxConcurrency", "1"),
            ]);
        });
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<ICoreContextFactory>();
            services.RemoveAll<ISecurityDbContextFactory>();

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
                        ["DecryptionKey"] = decryptionKey,
                        ["enableExternalEventing"] = "true",
                    },
                    Services = new Dictionary<string, string>(),
                });
            services.AddSingleton<ISecurityDbContextFactory>(
                _ => new MSSQLSecurityDbContextFactory(ssoConnectionString));
            services.AddCoreData(coreConnectionString);
        });
    }
}
