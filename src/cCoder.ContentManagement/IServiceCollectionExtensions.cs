// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Api.OData;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Events;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Exposures.Caching;
using cCoder.ContentManagement.Dependencies.Caching;
using cCoder.ContentManagement.Dependencies.Events;
using cCoder.ContentManagement.Exposures.EventHandlers;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Rendering.Services.Foundations;
using cCoder.ContentManagement.Rendering.Services.Orchestrations;
using cCoder.ContentManagement.Services;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Foundations;
using cCoder.ContentManagement.Services.Foundations.Authorization;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.ContentManagement.Services.Foundations.Exports;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using cCoder.Eventing;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi;

namespace cCoder.ContentManagement;

public static partial class IServiceCollectionExtensions
{
    public static void AddContentManagementWeb(
        this IServiceCollection services,
        Action<ContentManagementConfiguration> newContentManagementConfiguration = null,
        ODataConventionModelBuilder builder = null) =>
        services.AddConfiguredContentManagementWeb(newContentManagementConfiguration: (_, configuration) => newContentManagementConfiguration?.Invoke(obj: configuration), builder: builder);

    public static void AddContentManagementHostedServices(
        this IServiceCollection services,
        Action<ContentManagementConfiguration> newContentManagementConfiguration = null) =>
        services.AddConfiguredContentManagement(newContentManagementConfiguration: (_, configuration) => newContentManagementConfiguration?.Invoke(obj: configuration));

    private static void AddContentManagement(this IServiceCollection services)
    {
        services.AddEventingTypes();
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddCoordinations();
        services.AddEventHandlers();
        services.AddRendering();
    }

    private static void AddContentManagementWeb(this IServiceCollection services, ODataConventionModelBuilder builder = null) =>
        services.AddContentManagement();

    private static void AddEventingTypes(this IServiceCollection services)
    {
        services.AddEventingForType<App>();
        services.AddEventingForType<AppCulture>();
        services.AddEventingForType<CommonObject>();
        services.AddEventingForType<Component>();
        services.AddEventingForType<Content>();
        services.AddEventingForType<Culture>();
        services.AddEventingForType<Layout>();
        services.AddEventingForType<Package>();
        services.AddEventingForType<(int, Package)>();
        services.AddEventingForType<PackageItem>();
        services.AddEventingForType<Page>();
        services.AddEventingForType<PageInfo>();
        services.AddEventingForType<PageRole>();
        services.AddEventingForType<Resource>();
        services.AddEventingForType<Script>();
        services.AddEventingForType<Submission>();
        services.AddEventingForType<Template>();
    }

    private static void AddBrokers(this IServiceCollection services)
    {
        services.AddTransient<IEventHubBroker, EventHubBroker>();
        services.AddTransient<IEventInfrastructureDependency, EventInfrastructureDependency>();
        services.AddTransient<IAppCultureEventBroker, AppCultureEventBroker>();
        services.AddTransient<IAppEventBroker, AppEventBroker>();
        services.AddTransient<ICommonObjectEventBroker, CommonObjectEventBroker>();
        services.AddTransient<IComponentEventBroker, ComponentEventBroker>();
        services.AddTransient<IContentEventBroker, ContentEventBroker>();
        services.AddTransient<ICultureEventBroker, CultureEventBroker>();
        services.AddTransient<ILayoutEventBroker, LayoutEventBroker>();
        services.AddTransient<IPackageEventBroker, PackageEventBroker>();
        services.AddTransient<IPackageItemEventBroker, PackageItemEventBroker>();
        services.AddTransient<IPageEventBroker, PageEventBroker>();
        services.AddTransient<IPageInfoEventBroker, PageInfoEventBroker>();
        services.AddTransient<IPageRoleEventBroker, PageRoleEventBroker>();
        services.AddTransient<IResourceEventBroker, ResourceEventBroker>();
        services.AddTransient<IScriptEventBroker, ScriptEventBroker>();
        services.AddTransient<ISubmissionEventBroker, SubmissionEventBroker>();
        services.AddTransient<ITemplateEventBroker, TemplateEventBroker>();
        services.AddTransient<IAppBroker, AppBroker>();
        services.AddTransient<IAppCultureBroker, AppCultureBroker>();
        services.AddTransient<ICommonObjectBroker, CommonObjectBroker>();
        services.AddTransient<IComponentBroker, ComponentBroker>();
        services.AddTransient<IContentBroker, ContentBroker>();
        services.AddTransient<ICultureBroker, CultureBroker>();
        services.AddTransient<ILayoutBroker, LayoutBroker>();
        services.AddTransient<IPackageBroker, PackageBroker>();
        services.AddTransient<IPackageItemBroker, PackageItemBroker>();
        services.AddTransient<IPageBroker, PageBroker>();
        services.AddTransient<IPageInfoBroker, PageInfoBroker>();
        services.AddTransient<IPageRoleBroker, PageRoleBroker>();
        services.AddTransient<IPrivilegeBroker, PrivilegeBroker>();
        services.AddTransient<IRenderFileContentBroker, RenderFileContentBroker>();
        services.AddTransient<IResourceBroker, ResourceBroker>();
        services.AddTransient<IRoleBroker, RoleBroker>();
        services.AddTransient<IScriptBroker, ScriptBroker>();
        services.AddTransient<ISubmissionBroker, SubmissionBroker>();
        services.AddTransient<ITemplateBroker, TemplateBroker>();
        services.AddTransient<IAuthorizationBroker, AuthorizationBroker>();
        services.AddTransient<IJsonBroker, JsonBroker>();
        services.AddTransient<IUserRoleBroker, UserRoleBroker>();
    }

    private static void AddCoordinations(this IServiceCollection services)
    {
        services.AddTransient<IAppRenderableCoordinationService, AppRenderableCoordinationService>();
        services.AddTransient<IAppPageComponentCoordinationService, AppPageComponentCoordinationService>();
        services.AddTransient<IAppSupportingResourcesCoordinationService, AppSupportingResourcesCoordinationService>();
        services.AddTransient<IPageCoordinationService, PageCoordinationService>();
        services.AddTransient<IPageStructureCoordinationService, PageStructureCoordinationService>();
    }

    private static void AddEventHandlers(this IServiceCollection services)
    {
        services.AddTransient<IAppManager, AppManager>();
        services.AddTransient<IContentManagementPackageManager, ContentManagementPackageManager>();
        services.AddTransient<IComponentRenderer, ComponentRenderer>();
        services.AddTransient<IPageRenderer, PageRenderer>();
        services.AddTransient<ITemplateRenderer, TemplateRenderer>();
        services.AddTransient<IContentManagementEventHandlers, ContentManagementEventHandlers>();
    }

    private static void AddRendering(this IServiceCollection services)
    {
        services.AddTransient<IPageRenderCoordinationService, PageRenderCoordinationService>();
        services.AddTransient<IPageRenderOrchestrationService, PageRenderOrchestrationService>();
        services.AddTransient<IPageRenderProcessingService, PageRenderProcessingService>();
        services.AddTransient<IPageRenderExecutionOrchestrationService, PageRenderExecutionOrchestrationService>();
        services.AddTransient<IMetadataCacheService, MetadataCacheService>();
        services.AddTransient<ICommonObjectCacheService, CommonObjectCacheService>();
        services.AddTransient<IMarkupRenderService, MarkupRenderService>();
        services.AddTransient<IComponentReaderBroker, ComponentReaderBroker>();
        services.AddTransient<IScriptReaderBroker, ScriptReaderBroker>();
        services.AddTransient<IMetadataReaderBroker, MetadataReaderBroker>();
        services.AddTransient<ICommonObjectReaderBroker, CommonObjectReaderBroker>();
    }

    private static void AddFoundations(this IServiceCollection services)
    {
        services.AddTransient<IEventHandlerService, EventHandlerService>();
        services.AddTransient<IAuthorizationService, AuthorizationService>();
        services.AddTransient<IAppCultureEventService, AppCultureEventService>();
        services.AddTransient<IAppEventService, AppEventService>();
        services.AddTransient<ICommonObjectEventService, CommonObjectEventService>();
        services.AddTransient<IComponentEventService, ComponentEventService>();
        services.AddTransient<IContentEventService, ContentEventService>();
        services.AddTransient<ICultureEventService, CultureEventService>();
        services.AddTransient<ILayoutEventService, LayoutEventService>();
        services.AddTransient<IPackageEventService, PackageEventService>();
        services.AddTransient<IPackageItemEventService, PackageItemEventService>();
        services.AddTransient<IPageEventService, PageEventService>();
        services.AddTransient<IPageInfoEventService, PageInfoEventService>();
        services.AddTransient<IPageRoleEventService, PageRoleEventService>();
        services.AddTransient<IResourceEventService, ResourceEventService>();
        services.AddTransient<IScriptEventService, ScriptEventService>();
        services.AddTransient<ISubmissionEventService, SubmissionEventService>();
        services.AddTransient<ITemplateEventService, TemplateEventService>();
        services.AddTransient<IPackageExportService, PackageExportService>();
        services.AddTransient<IAppCultureService, AppCultureService>();
        services.AddTransient<IAppService, AppService>();
        services.AddTransient<ICommonObjectService, CommonObjectService>();
        services.AddTransient<IComponentService, ComponentService>();
        services.AddTransient<IContentService, ContentService>();
        services.AddTransient<ICultureService, CultureService>();
        services.AddTransient<ILayoutService, LayoutService>();
        services.AddTransient<IPackageItemService, PackageItemService>();
        services.AddTransient<IPackageService, PackageService>();
        services.AddTransient<IPageInfoService, PageInfoService>();
        services.AddTransient<IPageRoleService, PageRoleService>();
        services.AddTransient<IPageService, PageService>();
        services.AddTransient<IResourceService, ResourceService>();
        services.AddTransient<IScriptService, ScriptService>();
        services.AddTransient<ISubmissionService, SubmissionService>();
        services.AddTransient<ITemplateService, TemplateService>();
        services.AddTransient<
            ICurrentAppProcessingService,
            CurrentAppProcessingService>();

        services.AddTransient<ICurrentAppResolver, CurrentAppManager>();
        services.AddTransient<IContentManagementMetadataTypeService, ContentManagementMetadataTypeService>();
        services.AddTransient<IRenderFileContentService, RenderFileContentService>();
        services.AddTransient<IResourceProvider, CoreResourceBroker>();
        services.AddSingleton<ICommonObjectCache, CommonObjectCacheDependency>();
        services.AddSingleton<MetadataCacheDependency>();
        services.AddSingleton<IMetadataCache>(
            implementationFactory: serviceProvider =>
                serviceProvider.GetRequiredService<MetadataCacheDependency>());
    }

    private static void AddOrchestrations(this IServiceCollection services)
    {
        services.AddTransient<IContentManagementMigrationAggregationService, ContentManagementMigrationAggregationService>();
        services.AddTransient<IAppCultureOrchestrationService, AppCultureOrchestrationService>();
        services.AddTransient<IAppOrchestrationService, AppOrchestrationService>();
        services.AddTransient<ICommonObjectOrchestrationService, CommonObjectOrchestrationService>();
        services.AddTransient<IComponentOrchestrationService, ComponentOrchestrationService>();
        services.AddTransient<IComponentRenderOrchestrationService, ComponentRenderOrchestrationService>();
        services.AddTransient<IContentOrchestrationService, ContentOrchestrationService>();
        services.AddTransient<ICultureOrchestrationService, CultureOrchestrationService>();
        services.AddTransient<ILayoutOrchestrationService, LayoutOrchestrationService>();
        services.AddTransient<IPackageItemOrchestrationService, PackageItemOrchestrationService>();
        services.AddTransient<IPackageOrchestrationService, PackageOrchestrationService>();
        services.AddTransient<IPageInfoOrchestrationService, PageInfoOrchestrationService>();
        services.AddTransient<IPageOrchestrationService, PageOrchestrationService>();
        services.AddTransient<IPageRoleOrchestrationService, PageRoleOrchestrationService>();
        services.AddTransient<IResourceOrchestrationService, ResourceOrchestrationService>();
        services.AddTransient<IScriptOrchestrationService, ScriptOrchestrationService>();
        services.AddTransient<ISubmissionOrchestrationService, SubmissionOrchestrationService>();
        services.AddTransient<ITemplateOrchestrationService, TemplateOrchestrationService>();
        services.AddTransient<ITemplateRenderOrchestrationService, TemplateRenderOrchestrationService>();
    }

    private static void AddProcessings(this IServiceCollection services)
    {
        services.AddTransient<IAuthorizationProcessingService, AuthorizationProcessingService>();
        services.AddTransient<IAppCultureEventProcessingService, AppCultureEventProcessingService>();
        services.AddTransient<IAppCultureProcessingService, AppCultureProcessingService>();
        services.AddTransient<IAppEventProcessingService, AppEventProcessingService>();
        services.AddTransient<IAppProcessingService, AppProcessingService>();
        services.AddTransient<ICommonObjectEventProcessingService, CommonObjectEventProcessingService>();
        services.AddTransient<ICommonObjectProcessingService, CommonObjectProcessingService>();
        services.AddTransient<IComponentEventProcessingService, ComponentEventProcessingService>();
        services.AddTransient<IComponentProcessingService, ComponentProcessingService>();
        services.AddTransient<IComponentRenderProcessingService, ComponentRenderProcessingService>();
        services.AddTransient<IContentEventProcessingService, ContentEventProcessingService>();
        services.AddTransient<IContentProcessingService, ContentProcessingService>();
        services.AddTransient<ICultureEventProcessingService, CultureEventProcessingService>();
        services.AddTransient<ICultureProcessingService, CultureProcessingService>();
        services.AddTransient<ILayoutEventProcessingService, LayoutEventProcessingService>();
        services.AddTransient<ILayoutProcessingService, LayoutProcessingService>();
        services.AddTransient<IPackageEventProcessingService, PackageEventProcessingService>();
        services.AddTransient<IPackageExportProcessingService, PackageExportProcessingService>();
        services.AddTransient<IPackageItemEventProcessingService, PackageItemEventProcessingService>();
        services.AddTransient<IPackageItemProcessingService, PackageItemProcessingService>();
        services.AddTransient<IPackageProcessingService, PackageProcessingService>();
        services.AddTransient<IPageEventProcessingService, PageEventProcessingService>();
        services.AddTransient<IPageInfoEventProcessingService, PageInfoEventProcessingService>();
        services.AddTransient<IPageInfoProcessingService, PageInfoProcessingService>();
        services.AddTransient<IPageProcessingService, PageProcessingService>();
        services.AddTransient<IPageRoleEventProcessingService, PageRoleEventProcessingService>();
        services.AddTransient<IPageRoleProcessingService, PageRoleProcessingService>();
        services.AddTransient<IResourceEventProcessingService, ResourceEventProcessingService>();
        services.AddTransient<IResourceProcessingService, ResourceProcessingService>();
        services.AddTransient<IScriptEventProcessingService, ScriptEventProcessingService>();
        services.AddTransient<IScriptProcessingService, ScriptProcessingService>();
        services.AddTransient<ISubmissionEventProcessingService, SubmissionEventProcessingService>();
        services.AddTransient<ISubmissionProcessingService, SubmissionProcessingService>();
        services.AddTransient<ITemplateEventProcessingService, TemplateEventProcessingService>();
        services.AddTransient<ITemplateProcessingService, TemplateProcessingService>();
        services.AddTransient<ITemplateRenderProcessingService, TemplateRenderProcessingService>();
    }

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
        new ContentManagementModelBroker(builder: builder).Configure();

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
        bool useFullSchemaIds) =>
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