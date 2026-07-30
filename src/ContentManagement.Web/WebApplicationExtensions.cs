// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using System.Web;
using Apps.Shared;
using Apps.Shared.Hosting;
using cCoder.ContentManagement;
using cCoder.ContentManagement.Exposures;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Security.Data.EF.Dependencies;
using cCoder.Security.Data.EF;
using cCoder.Security.Models;
using cCoder.Security.Models.Entities;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.OData;
using Microsoft.Data.SqlClient;
using Microsoft.Net.Http.Headers;
namespace ContentManagement.Web;

public static class WebApplicationExtensions
{
    private static ILogger log = null!;
    private static string ssoConnection = string.Empty;

    public static WebApplication UseContentManagementApplication(
        this WebApplication app)
    {
        log = app.Services.GetRequiredService<ILogger<Program>>();
        ssoConnection = app.Services
            .GetRequiredService<SecurityConfiguration>()
            .ConnectionString;

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

        app.StartContentManagementWeb(onRequest: LogRequest, log: log);
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