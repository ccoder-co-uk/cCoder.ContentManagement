// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using System.Text.Json;
using cCoder.ContentManagement.Exposures.Caching;
using cCoder.ContentManagement.Exposures.EventHandlers;
using cCoder.ContentManagement.Services.Foundations;
using cCoder.Data.Exposures;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace cCoder.ContentManagement;

public static partial class WebApplicationExtensions
{
    private const string MetadataScope = "ContentManagement";

    public static WebApplication StartContentManagementWeb(
        this WebApplication app,
        ILogger log = null) =>
        app.UseContentManagementExposure(log: log)
        .ListenToContentManagementEvents();

    public static WebApplication StartContentManagementHostedServices(this WebApplication app) =>
        app.ListenToContentManagementEvents();

    private static WebApplication UseContentManagementExposure(
        this WebApplication app,
        ILogger log = null)
    {
        log?.LogInformation(message: "Initialising Content Management");
        app.UseSession();

        app.UseExceptionHandler(configure: errorApp =>
        {
            errorApp.Run(handler: async context =>
            {
                ILogger<IApplicationBuilder> appLogger = context.RequestServices.GetRequiredService<ILogger<IApplicationBuilder>>();
                Exception exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
                context.Response.StatusCode = ((exception?.GetType() == typeof(SecurityException)) ? 401 : 500);
                context.Response.ContentType = "application/json";

                if (exception != null)
                {
                    appLogger.LogError(message: "{Message}\n{StackTrace}", exception.Message, exception.StackTrace);
                    await context.Response.WriteAsync(text: "{ \"error\": \"" + exception.Message.Replace(oldValue: "\"", newValue: "'") + "\" }");
                }
            });
        });

        app.Use(middleware: (context, next) =>
        {
            Dictionary<string, StringValues> dictionary = QueryHelpers.ParseQuery(queryString: context.Request.QueryString.Value);

            if (dictionary.ContainsKey(key: "t"))
            {
                context.Request.Headers["Authorization"] = "bearer " + dictionary["t"][0];
            }

            if (dictionary.TryGetValue(key: "$format", value: out var value))
            {
                IHeaderDictionary headers = context.Request.Headers;
                string text = value[0];

                if (1 == 0)
                {
                }

                StringValues value2 = text switch
                {
                    "xml" => "application/xml",
                    "csv" => "text/csv",
                    "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    var ignoredFormat => context.Request.Headers["Content-Type"],
                };

                if (1 == 0)
                {
                }

                headers["Accept"] = value2;
                IHeaderDictionary headers2 = context.Response.Headers;
                string text2 = dictionary["$format"][0];

                if (1 == 0)
                {
                }

                string text3 = text2 switch
                {
                    "xml" => "attachment; filename=export.xml",
                    "csv" => "attachment; filename=export.csv",
                    "excel" => "attachment; filename=export.xlsx",
                    var ignoredExportFormat => "attachment; filename=export.json",
                };

                if (1 == 0)
                {
                }

                headers2["Content-Disposition"] = text3;
            }

            return next(context: context);
        });

        PopulateMetadataTypeCache(app: app);
        app.Services.GetService<ICommonObjectCache>()?.Refresh();
        app.Services.GetService<IMetadataCache>()?.Rebuild();

        app.Use(middleware: async (context, next) =>
        {
            context.Response.OnStarting(callback: () => RemovePlatformHeaders(context: context));
            await next(context: context);
        });

        return app;
    }

    private static void PopulateMetadataTypeCache(WebApplication app)
    {
        IMetadataTypeCache requiredService = app.Services.GetRequiredService<IMetadataTypeCache>();

        if (!requiredService.Contains(scope: "ContentManagement"))
        {
            requiredService.Set(
scope: "ContentManagement",
typeSetPayloads: app.Services.GetRequiredService<IContentManagementMetadataTypeService>()
                .GetKnownMetadata()
                .Select(selector: static metadata => JsonSerializer.Serialize(value: metadata)));
        }
    }

    private static WebApplication ListenToContentManagementEvents(this WebApplication app)
    {
        using IServiceScope serviceScope = app.Services.CreateScope();
        IServiceProvider serviceProvider = serviceScope.ServiceProvider;

        foreach (IContentManagementEventHandlers service in serviceProvider.GetServices<IContentManagementEventHandlers>())
        {
            service.ListenToAllEvents();
        }

        return app;
    }

    private static Task RemovePlatformHeaders(HttpContext context)
    {
        if (context.Request.Query["edit"] != "true")
        {
            context.Response.Headers.Append(key: "X-Frame-Options", value: "DENY");
        }

        context.Response.Headers.Remove(key: "X-AspNet-Version");
        context.Response.Headers.Remove(key: "X-AspNetMvc-Version");
        context.Response.Headers.Remove(key: "X-Sourcefiles");
        context.Response.Headers.Remove(key: "Server");
        return Task.CompletedTask;
    }

}