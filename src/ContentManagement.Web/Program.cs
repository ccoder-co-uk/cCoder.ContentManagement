// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using System.Web;
using Apps.Shared;
using cCoder.AppSecurity;
using cCoder.ContentManagement;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Security;
using cCoder.Security.Data.EF.Dependencies;
using cCoder.Security.Data.EF;
using cCoder.Security.Objects.Entities;
using cCoder.Eventing;
using cCoder.Eventing.Http;
using cCoder.Eventing.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.OData;
using Microsoft.Data.SqlClient;
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
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);

        string coreConnection = builder.Configuration.GetConnectionString(name: "Core")
            ?? throw new InvalidOperationException(message: "ConnectionStrings:Core is required.");

        ssoConnection = builder.Configuration.GetConnectionString(name: "SSO")
            ?? throw new InvalidOperationException(message: "ConnectionStrings:SSO is required.");

        CoreDataConfig config = new();
        builder.Configuration.Bind(instance: config);
        builder.Services.AddSingleton(implementationInstance: config);

        builder.Services.AddSingleton(
implementationInstance: new ContentManagementConfig
{
    ConnectionStrings = new Dictionary<string, string>(dictionary: config.ConnectionStrings),
    Settings = new Dictionary<string, string>(dictionary: config.Settings),
    Services = new Dictionary<string, string>(dictionary: config.Services),
    DebugInfo = config.DebugInfo,
    LogSQL = config.LogSQL,
});

        builder.Services.AddEventing();

        builder.Services.AddHttpEventingHostedServices(configure: options =>
        {
            options.MaxConcurrency =
                builder.Configuration.GetValue<int?>(key: "Eventing:Http:MaxConcurrency") ?? 1;
        });

        builder.Services.AddSecurityApi(configAction: (services, securityConfig) =>
        {
            securityConfig.AddMSSQLModelProvider(services: services, connectionString: ssoConnection);

            securityConfig.UseAESHMMACPasswordEncryption(
services: services,
decryptionKey: builder.Configuration.GetSection(key: "Settings")["DecryptionKey"]);
        });

        cCoder.Data.IServiceCollectionExtensions.AddCoreData(
services: builder.Services,
connectionString: coreConnection);

        builder.Services.AddAppSecurityWeb(configure: config =>
        {
            config.IncludeLegacyCoreContext = false;
        });

        builder.Services.AddContentManagementWeb(newContentManagementConfiguration: contentManagementConfiguration =>
            contentManagementConfiguration.WithEventProviders(
                CreateReceiveProvider<App>(eventNames: ["app_add", "app_update", "app_delete"]),
                CreateReceiveProvider<Page>(eventNames: ["page_add", "page_update", "page_delete"]),
                CreateReceiveProvider<(int appId, Package package)>(eventNames: ["package_import"])));

        builder.Logging.ClearProviders();

        builder.Logging.AddSimpleConsole(configure: options =>
        {
            options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss ";
            options.SingleLine = true;
        });

        WebApplication app = builder.Build();
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

        app.MapControllerRoute(
name: "default",
pattern: @"{*path}",
defaults: new { controller = "Home", action = "Index" },
constraints: new { path = new NoApiRouteConstraint() });

        app.StartContentManagementWeb(onRequest: LogRequest, log: log);
        app.UseDomainDefaultCors();
        app.UseDomainExceptionHandling(errorHandler: HandleUnhandledException);
        app.Run();
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

    private static EventProvider<T> CreateReceiveProvider<T>(string[] eventNames) =>
        new()
        {
            Events = eventNames,
            ReceiveHandler = async (serviceProvider, eventName, message) =>
            {
                IEventHub eventHub = serviceProvider.GetRequiredService<IEventHub>();

                await eventHub.RaiseEventAsync(
name: eventName,
message: new EventMessage<T>
{
    AuthInfo = new EventAuthInfo
    {
        SSOUserId = message.AuthInfo?.SSOUserId ?? "Guest",
    },
    Data = message.Data,
});
            },
        };

    private static async Task LogRequest(HttpContext context, ILogger logger)
    {
        HttpRequest request = context.RequestServices.GetService<HttpRequest>();

        if (request is null)
        {
            return;
        }

        using CoreDataContext core = context.RequestServices
            .GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        string ssoUserId = string.IsNullOrWhiteSpace(value: core.AuthInfo.SSOUserId)
            ? "Guest"
            : core.AuthInfo.SSOUserId;

        if (!await SqlUserExistsAsync(connectionString: ssoConnection, userId: ssoUserId, cancellationToken: context.RequestAborted))
        {
            ssoUserId = "Guest";
        }

        if (!await SqlUserExistsAsync(connectionString: ssoConnection, userId: ssoUserId, cancellationToken: context.RequestAborted))
        {
            return;
        }

        string url = HttpUtility.UrlDecode(str: request.GetDisplayUrl());
        string logEntry = $"{context.Connection.RemoteIpAddress} as {ssoUserId}: {request.Method} - {url}";

        if (await SqlTableExistsAsync(connectionString: ssoConnection, schema: "dbo", table: "UserEvents", cancellationToken: context.RequestAborted))
        {
            try
            {
                using var sso = new MSSQLSecurityDbContextFactory(connectionString: ssoConnection)
                    .CreateDbContext();

                string requestType = request.Path.Value?.StartsWith(value: "/api/", comparisonType: StringComparison.OrdinalIgnoreCase) == true
                    ? "Api_"
                    : "Page_";

                string tenantId = core.Apps.FirstOrDefault(predicate: app => app.Domain == request.Host.Host)?.TenantId;

                if (string.IsNullOrWhiteSpace(value: tenantId)
                    || !await SqlTenantExistsAsync(connectionString: ssoConnection, tenantId: tenantId, cancellationToken: context.RequestAborted))
                {
                    tenantId = null;
                }

                UserEvent userEvent = new()
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    CreatedBy = ssoUserId,
                    EventName = $"{requestType}{request.Method}{request.Path.Value}",
                    CreatedOn = DateTimeOffset.UtcNow,
                    Value = url,
                };

                await sso.AddAsync(entity: userEvent);
                await sso.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                Exception baseException = exception.GetBaseException();

                logger.LogWarning(
message: "Unable to persist request log entry to SSO for {SSOUserId}. {Message}",
                    ssoUserId,
                    baseException.Message);
            }
        }

        logger.LogDebug(message: logEntry);
    }

    private static async Task<bool> SqlTenantExistsAsync(
        string connectionString,
        string tenantId,
        CancellationToken cancellationToken)
    {
        try
        {
            SqlConnectionStringBuilder builder = new(connectionString: connectionString)
            {
                ConnectTimeout = 2,
            };

            await using SqlConnection connection = new(connectionString: builder.ConnectionString);
            await connection.OpenAsync(cancellationToken: cancellationToken);
            await using SqlCommand command = connection.CreateCommand();
            command.CommandTimeout = 2;
            command.CommandText = "SELECT 1 FROM dbo.Tenants WHERE Id = @tenantId";
            command.Parameters.AddWithValue(parameterName: "@tenantId", value: tenantId);

            object result = await command.ExecuteScalarAsync(cancellationToken: cancellationToken);
            return result is not null and not DBNull;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static async Task<bool> SqlUserExistsAsync(
        string connectionString,
        string userId,
        CancellationToken cancellationToken)
    {
        try
        {
            SqlConnectionStringBuilder builder = new(connectionString: connectionString)
            {
                ConnectTimeout = 2,
            };

            await using SqlConnection connection = new(connectionString: builder.ConnectionString);
            await connection.OpenAsync(cancellationToken: cancellationToken);
            await using SqlCommand command = connection.CreateCommand();
            command.CommandTimeout = 2;
            command.CommandText = "SELECT 1 FROM dbo.Users WHERE Id = @userId";
            command.Parameters.AddWithValue(parameterName: "@userId", value: userId);

            object result = await command.ExecuteScalarAsync(cancellationToken: cancellationToken);
            return result is not null and not DBNull;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static async Task<bool> SqlTableExistsAsync(
        string connectionString,
        string schema,
        string table,
        CancellationToken cancellationToken)
    {
        try
        {
            SqlConnectionStringBuilder builder = new(connectionString: connectionString)
            {
                ConnectTimeout = 2,
            };

            await using SqlConnection connection = new(connectionString: builder.ConnectionString);
            await connection.OpenAsync(cancellationToken: cancellationToken);
            await using SqlCommand command = connection.CreateCommand();
            command.CommandTimeout = 2;
            command.CommandText = "SELECT OBJECT_ID(@tableName, 'U')";
            command.Parameters.AddWithValue(parameterName: "@tableName", value: $"{schema}.{table}");

            object result = await command.ExecuteScalarAsync(cancellationToken: cancellationToken);
            return result is not null and not DBNull;
        }
        catch (Exception)
        {
            return false;
        }
    }
}