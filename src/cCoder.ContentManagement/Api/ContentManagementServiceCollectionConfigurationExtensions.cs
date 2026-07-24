// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Api.OData;
using cCoder.ContentManagement.Models;
using cCoder.Eventing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi;

namespace cCoder.ContentManagement;

public static partial class IServiceCollectionExtensions
{
    private static ContentManagementConfiguration AddConfiguredContentManagement(
        this IServiceCollection services,
        Action<IServiceCollection, ContentManagementConfiguration> newContentManagementConfiguration)
    {
        ContentManagementConfiguration configuration = CreateConfiguration(services: services, newContentManagementConfiguration: newContentManagementConfiguration);
        services.AddContentManagement();
        return configuration;
    }

    private static ContentManagementConfiguration AddConfiguredContentManagementWeb(
        this IServiceCollection services,
        Action<IServiceCollection, ContentManagementConfiguration> newContentManagementConfiguration,
        ODataConventionModelBuilder builder = null)
    {
        ContentManagementConfiguration configuration = CreateConfiguration(services: services, newContentManagementConfiguration: newContentManagementConfiguration);
        services.AddContentManagementWeb(builder: builder);

        services.AddConfiguredApi(
newContentManagementConfiguration: configuration,
documentName: "ContentManagement",
configureModel: static modelBuilder => modelBuilder.ConfigureContentManagementApiModel(),
builder: builder);

        return configuration;
    }

    public static void ConfigureContentManagementApiModel(this ODataConventionModelBuilder builder) =>
        new ContentManagementModelBuilder(builder: builder).Configure();

    private static ContentManagementConfiguration CreateConfiguration(
        IServiceCollection services,
        Action<IServiceCollection, ContentManagementConfiguration> newContentManagementConfiguration)
    {
        ContentManagementConfiguration configuration = new();
        newContentManagementConfiguration?.Invoke(arg1: services, arg2: configuration);
        services.AddSingleton(implementationInstance: configuration);
        services.AddEventProviders(eventProviders: configuration.EventProviders);
        return configuration;
    }

    private static void AddConfiguredApi(
        this IServiceCollection services,
        ContentManagementConfiguration newContentManagementConfiguration,
        string documentName,
        Action<ODataConventionModelBuilder> configureModel,
        ODataConventionModelBuilder builder = null,
        bool useFullSchemaIds = false)
    {
        services.AddSingleton<Action<ODataConventionModelBuilder>>(implementationInstance: configureModel);

        if (builder is not null)
        {
            configureModel(obj: builder);
        }

        AddAspNet(services: services);

        if (builder is null)
        {
            AddApiDocumentation(services: services, documentName: documentName, newContentManagementConfiguration: newContentManagementConfiguration, useFullSchemaIds: useFullSchemaIds);
        }

        IEdmModel routeModel = BuildRouteModel(configureModel: configureModel);
        DefaultODataBatchHandler batchHandler = new();

        string rootPath = string.IsNullOrWhiteSpace(value: newContentManagementConfiguration.RootPath)
            ? $"Api/{documentName}"
            : newContentManagementConfiguration.RootPath;

        services.AddControllers()
            .AddOData(setupAction: options =>
        {
            options.RouteOptions.EnableQualifiedOperationCall = false;
            options.EnableAttributeRouting = true;
            options.RouteOptions.EnableKeyAsSegment = false;

            options.Expand()
                .Count()
                .Filter()
                .Select()
                .OrderBy()
                .SetMaxTop(maxTopValue: 1000)
                .AddRouteComponents(routePrefix: rootPath, model: routeModel, batchHandler: batchHandler);

            if (builder is null
                && newContentManagementConfiguration.IncludeLegacyCoreContext
                && !string.Equals(a: rootPath, b: "Api/Core", comparisonType: StringComparison.OrdinalIgnoreCase))
            {
                options.AddRouteComponents(routePrefix: "Api/Core", model: routeModel, batchHandler: batchHandler);
            }
        });
    }

    private static void AddApiDocumentation(
        IServiceCollection services,
        string documentName,
        ContentManagementConfiguration newContentManagementConfiguration,
        bool useFullSchemaIds)
    {
        services.AddSwaggerGen(setupAction: options =>
        {
            options.ResolveConflictingActions(resolver: apiDescriptions => apiDescriptions.First());
            AddSwaggerDocuments(options: options, documentName: documentName, newContentManagementConfiguration: newContentManagementConfiguration);

            options.DocInclusionPredicate(
predicate: (swaggerDocumentName, apiDescription) =>
                    ShouldIncludeInDocument(
swaggerDocumentName: swaggerDocumentName,
relativePath: apiDescription.RelativePath,
documentName: documentName,
configuration: newContentManagementConfiguration));

            if (useFullSchemaIds)
            {
                options.CustomSchemaIds(schemaIdSelector: type => type.FullName?.Replace(oldChar: '+', newChar: '.') ?? type.Name);
            }

            //options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            //{
            //    Description = @"Authorization header using the Bearer scheme.",
            //    Name = "Authorization",
            //    In = ParameterLocation.Header,
            //    Type = SecuritySchemeType.ApiKey,
            //    Scheme = "bearer",
            //});
        });
    }

    private static void AddSwaggerDocuments(
        Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options,
        string documentName,
        ContentManagementConfiguration newContentManagementConfiguration)
    {
        options.SwaggerDoc(name: documentName, info: new OpenApiInfo
        {
            Title = $"{documentName} API definition",
            Version = documentName,
        });

        if (newContentManagementConfiguration.IncludeLegacyCoreContext)
        {
            options.SwaggerDoc(name: "Core", info: new OpenApiInfo
            {
                Title = "Core API definition",
                Version = "Core",
            });

            options.SwaggerDoc(name: "v1", info: new OpenApiInfo
            {
                Title = "Core API definition",
                Version = "v1",
            });
        }
    }

    private static bool ShouldIncludeInDocument(
        string swaggerDocumentName,
        string relativePath,
        string documentName,
        ContentManagementConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(value: relativePath))
        {
            return false;
        }

        if (string.Equals(a: swaggerDocumentName, b: "v1", comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            swaggerDocumentName = "Core";
        }

        string path = NormalizePath(relativePath: relativePath);

        string rootPath = string.IsNullOrWhiteSpace(value: configuration.RootPath)
            ? $"Api/{documentName}"
            : configuration.RootPath;

        return string.Equals(a: swaggerDocumentName, b: "Core", comparisonType: StringComparison.OrdinalIgnoreCase)
            ? configuration.IncludeLegacyCoreContext && MatchesContextRoute(path: path, rootPath: "Api/Core")
            : MatchesContextRoute(path: path, rootPath: rootPath);
    }

    private static bool MatchesContextRoute(string path, string rootPath)
    {
        string normalizedPath = NormalizePath(relativePath: rootPath);

        return path.Equals(value: normalizedPath, comparisonType: StringComparison.OrdinalIgnoreCase)
            || path.StartsWith(value: $"{normalizedPath}/", comparisonType: StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizePath(string relativePath) =>
        relativePath.StartsWith(value: '/') ? relativePath : $"/{relativePath}";

    private static IEdmModel BuildRouteModel(Action<ODataConventionModelBuilder> configureModel)
    {
        ODataConventionModelBuilder builder = new();
        configureModel(obj: builder);
        return builder.GetEdmModel();
    }

    private static void AddAspNet(IServiceCollection services)
    {
        services.AddRouting();
        services.AddResponseCompression();
        services.AddHttpClient();
        services.AddHttpContextAccessor();

        services.AddScoped(
serviceType: typeof(HttpContext),
implementationFactory: ctx => ctx.GetService<IHttpContextAccessor>()?.HttpContext ?? new DefaultHttpContext());

        services.AddScoped(serviceType: typeof(HttpRequest), implementationFactory: ctx => ctx.GetRequiredService<HttpContext>()
            .Request);

        services.AddSession();

        services.AddHsts(configureOptions: options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromMinutes(minutes: 60);
        });

        services.AddMvc(setupAction: options => options.EnableEndpointRouting = false);
        services.AddRazorPages();

        services.Configure<KestrelServerOptions>(configureOptions: options =>
        {
            options.Limits.MaxRequestBodySize = int.MaxValue;
        });

        services.AddEndpointsApiExplorer();
        services.AddSignalR();
    }
}