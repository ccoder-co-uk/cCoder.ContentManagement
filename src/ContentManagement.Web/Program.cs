using System.Security;
using System.Web;
using Apps.Shared;
using cCoder.AppSecurity;
using cCoder.ContentManagement;
using cCoder.Data;
using cCoder.Security;
using cCoder.Security.Api;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.MSSQL;
using cCoder.Eventing;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.OData;
using Microsoft.Net.Http.Headers;
using ContentManagementConfig = cCoder.ContentManagement.Models.Config;
using CoreDataConfig = cCoder.Data.Config;


namespace ContentManagement.Web;

public class Program
{
    private static ILogger log = null!;
    private static string ssoConnection = string.Empty;

    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        string coreConnection = builder.Configuration.GetConnectionString("Core")
            ?? throw new InvalidOperationException("ConnectionStrings:Core is required.");

        ssoConnection = builder.Configuration.GetConnectionString("SSO")
            ?? throw new InvalidOperationException("ConnectionStrings:SSO is required.");

        CoreDataConfig config = new();
        builder.Configuration.Bind(config);
        builder.Services.AddSingleton(config);
        builder.Services.AddSingleton(
            new ContentManagementConfig
            {
                ConnectionStrings = new Dictionary<string, string>(config.ConnectionStrings),
                Settings = new Dictionary<string, string>(config.Settings),
                Services = new Dictionary<string, string>(config.Services),
                DebugInfo = config.DebugInfo,
                LogSQL = config.LogSQL,
            });

        builder.Services.AddEventing();
        builder.Services.AddSecurityApi((services, securityConfig) =>
        {
            securityConfig.AddMSSQLModelProvider(services, ssoConnection);
            securityConfig.UseAESHMMACPasswordEncryption(
                services,
                builder.Configuration.GetSection("Settings")["DecryptionKey"]);
        });
        cCoder.Data.IServiceCollectionExtensions.AddCoreData(
            builder.Services,
            coreConnection);
        builder.Services.AddAppSecurityHostedServices();
        builder.Services.AddContentManagementWeb();

        builder.Logging.ClearProviders();
        builder.Logging.AddSimpleConsole(options =>
        {
            options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss ";
            options.SingleLine = true;
        });

        WebApplication app = builder.Build();
        log = app.Services.GetRequiredService<ILogger<Program>>();

        app.UseHttpsRedirection();
        app.UseSession();
        app.UseStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = context =>
                context.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=86400",
        });

        app.UseSwagger()
            .UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/ContentManagement/swagger.json", "ContentManagement API");
                options.SwaggerEndpoint("/swagger/Core/swagger.json", "Core API");
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Core API");
            })
            .UseODataBatching()
            .UseODataRouteDebug();

        app.UseDomainApiShell();
        app.MapControllerRoute(
            name: "default",
            pattern: @"{*path}",
            defaults: new { controller = "Home", action = "Index" },
            constraints: new { path = new NoApiRouteConstraint() });
        app.StartContentManagementWeb(LogRequest, log);
        app.UseDomainDefaultCors();
        app.UseDomainExceptionHandling(HandleUnhandledException);
        app.StartAppSecurityHostedServices();
        app.Run();
    }

    private static async Task HandleUnhandledException(HttpContext context)
    {
        Exception exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

        context.Response.StatusCode =
            exception?.GetType() == typeof(SecurityException) ? 401 : 500;
        context.Response.ContentType = "application/json";

        if (exception is null)
            return;

        log.LogError("{Message}\n{StackTrace}", exception.Message, exception.StackTrace);
    await context.Response.WriteAsync(
        "{ \"error\": \"" + exception.Message.Replace("\"", "\'") + "\" }");
}

    private static async Task LogRequest(HttpContext context, ILogger logger)
    {
        HttpRequest request = context.RequestServices.GetService<HttpRequest>();

        if (request is null)
            return;

        using CoreDataContext core = context.RequestServices
            .GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();
        string url = HttpUtility.UrlDecode(request.GetDisplayUrl());
        string logEntry = $"{context.Connection.RemoteIpAddress} as {core.AuthInfo.SSOUserId}: {request.Method} - {url}";

        if (context.Session is not null)
        {
            try
            {
                using var sso = new MSSQLSecurityDbContextFactory(ssoConnection)
                    .CreateDbContext();

                string requestType = request.Path.Value?.StartsWith("/api/", StringComparison.OrdinalIgnoreCase) == true
                    ? "Api_"
                    : "Page_";

                object userEvent = CreateUserEvent(
                    tenantId: core.Apps.FirstOrDefault(app => app.Domain == request.Host.Host)?.TenantId,
                    createdBy: core.AuthInfo.SSOUserId,
                    eventName: $"{requestType}{request.Method}{request.Path.Value}",
                    sessionId: context.Session.Id,
                    value: url);
                await sso.AddAsync(userEvent);
                await sso.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                logger.LogWarning(
                    "Unable to persist request log entry to SSO. {Message}",
                    exception.Message);
            }
        }

        logger.LogDebug(logEntry);
    }

    private static object CreateUserEvent(
        string tenantId,
        string createdBy,
        string eventName,
        string sessionId,
        string value)
    {
        Type userEventType =
            Type.GetType("cCoder.Security.Objects.Entities.UserEvent, cCoder.Security.Data", false)
            ?? Type.GetType("cCoder.Security.Objects.Entities.UserEvent, cCoder.Security.Objects", false)
            ?? throw new InvalidOperationException("Unable to resolve the security UserEvent type.");

        object userEvent = Activator.CreateInstance(userEventType)
            ?? throw new InvalidOperationException("Unable to construct the security UserEvent type.");

        userEventType.GetProperty("TenantId")?.SetValue(userEvent, tenantId);
        userEventType.GetProperty("CreatedBy")?.SetValue(userEvent, createdBy);
        userEventType.GetProperty("EventName")?.SetValue(userEvent, eventName);
        userEventType.GetProperty("CreatedOn")?.SetValue(userEvent, DateTimeOffset.UtcNow);
        userEventType.GetProperty("SessionId")?.SetValue(userEvent, sessionId);
        userEventType.GetProperty("Value")?.SetValue(userEvent, value);
        return userEvent;
    }
}



