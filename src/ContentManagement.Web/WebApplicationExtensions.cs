// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using Apps.Shared;
using Apps.Shared.Hosting;
using cCoder.ContentManagement;
using cCoder.ContentManagement.Exposures;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.OData;
using Microsoft.Net.Http.Headers;
namespace ContentManagement.Web;

public static class WebApplicationExtensions
{
    private static ILogger log = null!;

    public static WebApplication UseContentManagementApplication(
        this WebApplication app)
    {
        log = app.Services.GetRequiredService<ILogger<Program>>();

        app.UseHttpsRedirection();
        app.UseSession();

        app.UseStaticFiles(options: new StaticFileOptions
        {
            OnPrepareResponse = context =>
                context.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=86400",
        });

        app.UseSwagger()
            .UseSwaggerUI(setupAction: options =>
            {
                options.SwaggerEndpoint(url: "/swagger/ContentManagement/swagger.json", name: "ContentManagement API");
                options.SwaggerEndpoint(url: "/swagger/Core/swagger.json", name: "Core API");
                options.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "Core API");
            })
            .UseODataBatching()
            .UseODataRouteDebug();

        app.UseDomainApiShell();
        app.MapGet(pattern: "/Health", handler: () => Results.Text(content: "OK"));
        app.MapGet(pattern: "/", handler: () => Results.Redirect(url: "/tools/index.html"));

        app.StartContentManagementWeb(log: log);
        app.UseDomainDefaultCors();
        app.UseDomainExceptionHandling(errorHandler: HandleUnhandledException);
        return app;
    }

    public static void UseDomainApiShell(this WebApplication app)
    {
        app.UseRouting();
        app.MapControllers();
    }

    public static void UseDomainDefaultCors(this WebApplication app)
    {
        app.UseCors(configurePolicy: builder =>
        {
            builder.AllowAnyHeader();
            builder.AllowAnyMethod();
            builder.AllowAnyOrigin();
        });
    }

    public static void UseDomainExceptionHandling(
        this WebApplication app,
        RequestDelegate errorHandler)
    {
        app.UseExceptionHandler(
            configure: errorApp =>
                errorApp.Run(handler: errorHandler));
    }

    private static async Task HandleUnhandledException(HttpContext context)
    {
        Exception exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

        context.Response.StatusCode =
            exception?.GetType() == typeof(SecurityException) ? 401 : 500;

        context.Response.ContentType = "application/json";

        if (exception is null)
        {
            return;
        }

        log.LogError(message: "{Message}\n{StackTrace}", exception.Message, exception.StackTrace);

        await context.Response.WriteAsync(
text: "{ \"error\": \"" + exception.Message.Replace(oldValue: "\"", newValue: "\'") + "\" }");
    }

}