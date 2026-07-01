using System.Security;
using System.Web;
using Apps.Shared;
using cCoder.AppSecurity;
using cCoder.ContentManagement;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Security;
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
        builder.Services.AddHttpEventingHostedServices(options =>
        {
            options.MaxConcurrency =
                builder.Configuration.GetValue<int?>("Eventing:Http:MaxConcurrency") ?? 1;
        });

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
        builder.Services.AddAppSecurityWeb(config =>
        {
            config.IncludeLegacyCoreContext = false;
        });

        builder.Services.AddContentManagementWeb(contentManagementConfiguration =>
            contentManagementConfiguration.WithEventProviders(
                CreateReceiveProvider<App>(["app_add", "app_update", "app_delete"]),
                CreateReceiveProvider<Page>(["page_add", "page_update", "page_delete"]),
                CreateReceiveProvider<(int appId, Package package)>(["package_import"])));

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
        app.MapGet("/Health", () => Results.Text("OK"));
        app.MapGet("/", () => Results.Redirect("/tools/index.html"));
        app.MapControllerRoute(
            name: "default",
            pattern: @"{*path}",
            defaults: new { controller = "Home", action = "Index" },
            constraints: new { path = new NoApiRouteConstraint() });
        app.StartContentManagementWeb(LogRequest, log);
        app.UseDomainDefaultCors();
        app.UseDomainExceptionHandling(HandleUnhandledException);
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

    private static EventProvider<T> CreateReceiveProvider<T>(string[] eventNames) =>
        new()
        {
            Events = eventNames,
            ReceiveHandler = async (serviceProvider, eventName, message) =>
            {
                IEventHub eventHub = serviceProvider.GetRequiredService<IEventHub>();

                await eventHub.RaiseEventAsync(
                    eventName,
                    new EventMessage<T>
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
            return;

        using CoreDataContext core = context.RequestServices
            .GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();
        string ssoUserId = string.IsNullOrWhiteSpace(core.AuthInfo.SSOUserId)
            ? "Guest"
            : core.AuthInfo.SSOUserId;

        if (!await SqlUserExistsAsync(ssoConnection, ssoUserId, context.RequestAborted))
            ssoUserId = "Guest";

        if (!await SqlUserExistsAsync(ssoConnection, ssoUserId, context.RequestAborted))
            return;

        string url = HttpUtility.UrlDecode(request.GetDisplayUrl());
        string logEntry = $"{context.Connection.RemoteIpAddress} as {ssoUserId}: {request.Method} - {url}";

        if (await SqlTableExistsAsync(ssoConnection, "dbo", "UserEvents", context.RequestAborted))
        {
            try
            {
                using var sso = new MSSQLSecurityDbContextFactory(ssoConnection)
                    .CreateDbContext();

                string requestType = request.Path.Value?.StartsWith("/api/", StringComparison.OrdinalIgnoreCase) == true
                    ? "Api_"
                    : "Page_";
                string tenantId = core.Apps.FirstOrDefault(app => app.Domain == request.Host.Host)?.TenantId;

                if (string.IsNullOrWhiteSpace(tenantId)
                    || !await SqlTenantExistsAsync(ssoConnection, tenantId, context.RequestAborted))
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

                await sso.AddAsync(userEvent);
                await sso.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                Exception baseException = exception.GetBaseException();

                logger.LogWarning(
                    "Unable to persist request log entry to SSO for {SSOUserId}. {Message}",
                    ssoUserId,
                    baseException.Message);
            }
        }

        logger.LogDebug(logEntry);
    }

    private static async Task<bool> SqlTenantExistsAsync(
        string connectionString,
        string tenantId,
        CancellationToken cancellationToken)
    {
        try
        {
            SqlConnectionStringBuilder builder = new(connectionString)
            {
                ConnectTimeout = 2,
            };

            await using SqlConnection connection = new(builder.ConnectionString);
            await connection.OpenAsync(cancellationToken);
            await using SqlCommand command = connection.CreateCommand();
            command.CommandTimeout = 2;
            command.CommandText = "SELECT 1 FROM dbo.Tenants WHERE Id = @tenantId";
            command.Parameters.AddWithValue("@tenantId", tenantId);

            object result = await command.ExecuteScalarAsync(cancellationToken);
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
            SqlConnectionStringBuilder builder = new(connectionString)
            {
                ConnectTimeout = 2,
            };

            await using SqlConnection connection = new(builder.ConnectionString);
            await connection.OpenAsync(cancellationToken);
            await using SqlCommand command = connection.CreateCommand();
            command.CommandTimeout = 2;
            command.CommandText = "SELECT 1 FROM dbo.Users WHERE Id = @userId";
            command.Parameters.AddWithValue("@userId", userId);

            object result = await command.ExecuteScalarAsync(cancellationToken);
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
            SqlConnectionStringBuilder builder = new(connectionString)
            {
                ConnectTimeout = 2,
            };

            await using SqlConnection connection = new(builder.ConnectionString);
            await connection.OpenAsync(cancellationToken);
            await using SqlCommand command = connection.CreateCommand();
            command.CommandTimeout = 2;
            command.CommandText = "SELECT OBJECT_ID(@tableName, 'U')";
            command.Parameters.AddWithValue("@tableName", $"{schema}.{table}");

            object result = await command.ExecuteScalarAsync(cancellationToken);
            return result is not null and not DBNull;
        }
        catch (Exception)
        {
            return false;
        }
    }
}



