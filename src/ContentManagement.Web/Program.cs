// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace ContentManagement.Web;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder =
            WebApplication.CreateBuilder(
                args: args);

        builder.Services.AddContentManagementWeb(
            configuration: builder.Configuration,
            configure: configuration =>
                configuration.ContentManagement.EventProviders =
                [
                    CreateReceiveProvider<cCoder.Data.Models.CMS.App>(
                        eventNames: ["app_add", "app_update", "app_delete"]),
                    CreateReceiveProvider<cCoder.Data.Models.CMS.Page>(
                        eventNames: ["page_add", "page_update", "page_delete"]),
                    CreateReceiveProvider<(
                        int appId,
                        cCoder.Data.Models.Packaging.Package package)>(
                            eventNames: ["package_import"])
                ]);

        WebApplication app = builder.Build();

        app.UseContentManagementApplication()
            .Run();
    }

    private static cCoder.Eventing.Models.EventProvider<T>
        CreateReceiveProvider<T>(
            string[] eventNames) =>
        new()
        {
            Events = eventNames,
            ReceiveHandler = async (
                serviceProvider,
                eventName,
                message) =>
            {
                cCoder.Eventing.IEventHub eventHub =
                    serviceProvider.GetRequiredService<
                        cCoder.Eventing.IEventHub>();

                await eventHub.RaiseEventAsync(
                    name: eventName,
                    message: new cCoder.Eventing.Models.EventMessage<T>
                    {
                        AuthInfo =
                            new cCoder.Eventing.Models.EventAuthInfo
                            {
                                SSOUserId =
                                    message.AuthInfo?.SSOUserId ?? "Guest",
                            },
                        Data = message.Data,
                    });
            },
        };
}