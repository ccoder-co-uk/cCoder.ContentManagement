using System.Security;
using System.Text.Json;
using cCoder.ContentManagement.Api.OData;
using cCoder.ContentManagement.Exposures.Caching;
using cCoder.ContentManagement.Exposures.EventHandlers;
using cCoder.ContentManagement.Services.Foundations;
using cCoder.Data.Exposures;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;

namespace cCoder.ContentManagement;

public static class WebApplicationExtensions
{
    private const string MetadataScope = "ContentManagement";

    public static WebApplication UseContentManagementExposure(this WebApplication app, Func<HttpContext, ILogger, Task> onRequest, ILogger log = null)
    {
        log?.LogInformation("Initialising Content Management");
        app.UseSession();
        app.UseExceptionHandler(delegate (IApplicationBuilder errorApp)
        {
            errorApp.Run(async delegate (HttpContext context)
            {
                ILogger<IApplicationBuilder> appLogger = context.RequestServices.GetRequiredService<ILogger<IApplicationBuilder>>();
                Exception exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
                context.Response.StatusCode = ((exception?.GetType() == typeof(SecurityException)) ? 401 : 500);
                context.Response.ContentType = "application/json";
                if (exception != null)
                {
                    appLogger.LogError("{Message}\n{StackTrace}", exception.Message, exception.StackTrace);
                    await context.Response.WriteAsync("{ \"error\": \"" + exception.Message.Replace("\"", "'") + "\" }");
                }
            });
        });
        app.Use(delegate (HttpContext context, Func<Task> next)
        {
            Dictionary<string, StringValues> dictionary = QueryHelpers.ParseQuery(context.Request.QueryString.Value);
            if (dictionary.ContainsKey("t"))
            {
                context.Request.Headers["Authorization"] = "bearer " + dictionary["t"][0];
            }
            if (dictionary.TryGetValue("$format", out var value))
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
                    _ => context.Request.Headers["Content-Type"],
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
                    _ => "attachment; filename=export.json",
                };
                if (1 == 0)
                {
                }
                headers2["Content-Disposition"] = text3;
            }
            return next();
        });
        PopulateMetadataTypeCache(app);
        app.Services.GetService<ICommonObjectCache>()?.Refresh();
        app.Services.GetService<IMetadataCache>()?.Rebuild();
        app.Use(async delegate (HttpContext context, Func<Task> next)
        {
            await onRequest(context, log ?? NullLogger.Instance);
            context.Response.OnStarting(() => RemovePlatformHeaders(context));
            await next();
        });
        return app;
    }

    private static void PopulateMetadataTypeCache(WebApplication app)
    {
        IMetadataTypeCache requiredService = app.Services.GetRequiredService<IMetadataTypeCache>();
        if (!requiredService.Contains("ContentManagement"))
        {
            requiredService.Set(
                "ContentManagement",
                app.Services.GetRequiredService<IContentManagementMetadataTypeService>()
                    .GetKnownMetadata()
                    .Select(static metadata => JsonSerializer.Serialize(metadata)));
        }
    }

    public static WebApplication ListenToContentManagementEvents(this WebApplication app)
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
            context.Response.Headers.Append("X-Frame-Options", "DENY");
        }
        context.Response.Headers.Remove("X-AspNet-Version");
        context.Response.Headers.Remove("X-AspNetMvc-Version");
        context.Response.Headers.Remove("X-Sourcefiles");
        context.Response.Headers.Remove("Server");
        return Task.CompletedTask;
    }

}

