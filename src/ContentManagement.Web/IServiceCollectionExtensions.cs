// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity;
using cCoder.ContentManagement;
using cCoder.Data;
using cCoder.Eventing;
using cCoder.Eventing.Http;
using cCoder.Eventing.Http.Models;
using cCoder.Security;
using ContentManagement.Web.Models;

namespace ContentManagement.Web;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddContentManagementWeb(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<ContentManagementWebConfiguration> configure = null)
    {
        ContentManagementWebConfiguration webConfiguration = new();
        configuration.Bind(instance: webConfiguration);
        configure?.Invoke(obj: webConfiguration);

        services.AddEventingWeb(configuration: webConfiguration.Eventing);
        services.AddHttpEventingHostedServices(
            configuration: new HttpEventingOptions
            {
                HubUrl = webConfiguration.Eventing.Http.HubUrl,
                MaxConcurrency =
                    webConfiguration.Eventing.Http.MaxConcurrency,
                JsonSerializerOptions =
                    new System.Text.Json.JsonSerializerOptions(
                        System.Text.Json.JsonSerializerDefaults.Web)
            });
        services.AddData(configuration: webConfiguration.Data);
        services.AddSecurityWeb(configuration: webConfiguration.Security);
        services.AddAppSecurityWeb(
            configuration: webConfiguration.AppSecurity);
        services.AddContentManagementWeb(
            configuration: webConfiguration.ContentManagement);

        return services;
    }
}