// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Extensions.OData;
using cCoder.ContentManagement.Extensions;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Events;
using cCoder.ContentManagement.Brokers.Authorizations;
using cCoder.ContentManagement.Brokers.HttpContexts;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Brokers.ServiceProviders;
using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Exposures.Caching;
using cCoder.ContentManagement.Dependencies.Caching;
using cCoder.ContentManagement.Dependencies.Events;
using cCoder.ContentManagement.Dependencies;
using cCoder.ContentManagement.Exposures.EventHandlers;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Rendering.Services.Foundations;
using cCoder.ContentManagement.Rendering.Services.Orchestrations;
using cCoder.ContentManagement.Services;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Foundations;
using cCoder.ContentManagement.Services.Foundations.Authorization;
using cCoder.ContentManagement.Services.Foundations.Authorizations;
using cCoder.ContentManagement.Services.Foundations.HttpContexts;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.ContentManagement.Services.Foundations.Exports;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data;
using cCoder.ContentManagement.Services.Foundations.Serialization;
using cCoder.ContentManagement.Services.Foundations.ServiceProviders;
using cCoder.ContentManagement.Services.Foundations.Rendering;
using cCoder.ContentManagement.Services.Processings.PageRendering;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Orchestrations.PageContexts;
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
        ODataConventionModelBuilder builder = null)
    {
        ContentManagementConfiguration configuration = new();
        newContentManagementConfiguration?.Invoke(obj: configuration);
        services.AddContentManagementWeb(configuration: configuration, builder: builder);
    }

    public static void AddContentManagementWeb(
        this IServiceCollection services,
        ContentManagementConfiguration configuration,
        ODataConventionModelBuilder builder = null)
    {
        services.RegisterConfiguration(configuration: configuration);
        services.AddEventingTypes();
        services.AddDependencies();
        services.AddBrokers();
        services.AddServiceProviderDependencies();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddCoordinations();
        services.AddEventHandlers();
        services.AddRendering();
        services.AddConfiguredApi(
            newContentManagementConfiguration: configuration,
            documentName: "ContentManagement",
            configureModel: static modelBuilder =>
                modelBuilder.ConfigureContentManagementApiModel(),
            builder: builder);
    }

    public static void AddContentManagementHostedServices(
        this IServiceCollection services,
        Action<ContentManagementConfiguration> newContentManagementConfiguration = null)
    {
        ContentManagementConfiguration configuration = new();
        newContentManagementConfiguration?.Invoke(obj: configuration);
        services.AddContentManagementHostedServices(configuration: configuration);
    }

    public static void AddContentManagementHostedServices(
        this IServiceCollection services,
        ContentManagementConfiguration configuration)
    {
        services.RegisterConfiguration(configuration: configuration);
        services.AddEventingTypes();
        services.AddDependencies();
        services.AddBrokers();
        services.AddServiceProviderDependencies();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddCoordinations();
        services.AddEventHandlers();
        services.AddRendering();
    }

    private static void AddServiceProviderDependencies(
        this IServiceCollection services)
    {
        services.AddTransient<IServiceProviderBroker, ServiceProviderBroker>();
        services.AddTransient<
            IServiceProviderExecutionService,
            ServiceProviderExecutionService>();

        services.AddKeyedTransient<IComponentOrchestrationService>(
            serviceKey: "Component",
            implementationFactory: (serviceProvider, _) =>
                serviceProvider.GetRequiredService<IComponentOrchestrationService>());

        services.AddKeyedTransient<IAppOrchestrationService>(
            serviceKey: "App",
            implementationFactory: (serviceProvider, _) =>
                serviceProvider.GetRequiredService<IAppOrchestrationService>());

        services.AddKeyedTransient<IAppUserProcessingService>(
            serviceKey: "AppUser",
            implementationFactory: (serviceProvider, _) =>
                serviceProvider.GetRequiredService<IAppUserProcessingService>());

        services.AddKeyedTransient<IComponentRenderOrchestrationService>(
            serviceKey: "ComponentRender",
            implementationFactory: (serviceProvider, _) =>
                serviceProvider.GetRequiredService<IComponentRenderOrchestrationService>());

        services.AddKeyedTransient<IPageOrchestrationService>(
            serviceKey: "Page",
            implementationFactory: (serviceProvider, _) =>
                serviceProvider.GetRequiredService<IPageOrchestrationService>());

        services.AddKeyedTransient<IPageRenderAggregationService>(
            serviceKey: "PageRender",
            implementationFactory: (serviceProvider, _) =>
                serviceProvider.GetRequiredService<IPageRenderAggregationService>());

        services.AddKeyedTransient<IPageRenderExecutionOrchestrationService>(
            serviceKey: "PageRenderExecution",
            implementationFactory: (serviceProvider, _) =>
                serviceProvider.GetRequiredService<IPageRenderExecutionOrchestrationService>());

        services.AddKeyedTransient<IAppService>(
            serviceKey: "AppStorage",
            implementationFactory: (serviceProvider, _) =>
                serviceProvider.GetRequiredService<IAppService>());

        services.AddKeyedTransient<IComponentService>(
            serviceKey: "ComponentStorage",
            implementationFactory: (serviceProvider, _) =>
                serviceProvider.GetRequiredService<IComponentService>());

        services.AddKeyedTransient<IResourceService>(
            serviceKey: "ResourceStorage",
            implementationFactory: (serviceProvider, _) =>
                serviceProvider.GetRequiredService<IResourceService>());

        services.AddKeyedTransient<IScriptService>(
            serviceKey: "ScriptStorage",
            implementationFactory: (serviceProvider, _) =>
                serviceProvider.GetRequiredService<IScriptService>());

        services.AddKeyedTransient<ITemplateService>(
            serviceKey: "TemplateStorage",
            implementationFactory: (serviceProvider, _) =>
                serviceProvider.GetRequiredService<ITemplateService>());

        services.AddKeyedTransient<IRenderFileContentService>(
            serviceKey: "RenderFileContent",
            implementationFactory: (serviceProvider, _) =>
                serviceProvider.GetRequiredService<IRenderFileContentService>());

        services.AddKeyedTransient<ITemplateOrchestrationService>(
            serviceKey: "Template",
            implementationFactory: (serviceProvider, _) =>
                serviceProvider.GetRequiredService<ITemplateOrchestrationService>());

        services.AddKeyedTransient<ITemplateContentBroker>(
            serviceKey: "TemplateContent",
            implementationFactory: (serviceProvider, _) =>
                serviceProvider.GetRequiredService<ITemplateContentBroker>());

        services.AddKeyedTransient<ITemplateRenderOrchestrationService>(
            serviceKey: "TemplateRender",
            implementationFactory: (serviceProvider, _) =>
                serviceProvider.GetRequiredService<ITemplateRenderOrchestrationService>());
    }

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
        services.AddEventingForType<PageRenderCacheMiss>();
        services.AddEventingForType<PageInfo>();
        services.AddEventingForType<PageRole>();
        services.AddEventingForType<Resource>();
        services.AddEventingForType<Script>();
        services.AddEventingForType<Submission>();
        services.AddEventingForType<Template>();
    }

    private static void AddDependencies(this IServiceCollection services)
    {
        services.AddTransient<TemplateContentDependency>();
        services.AddTransient<WorkflowExecutionDependency>();
    }

    private static void AddBrokers(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddTransient<IHttpContextBroker>(
            implementationFactory: serviceProvider =>
                new HttpContextBroker(
                    httpContext: serviceProvider
                        .GetRequiredService<IHttpContextAccessor>()
                        .HttpContext));
        services.AddTransient<IPageAuthorizationBroker, PageAuthorizationBroker>();
        services.AddTransient<IAuthenticatedEventHub, AuthenticatedEventHubDependency>();
        services.AddTransient<IEventHubBroker, EventHubBroker>();
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
        services.AddTransient<IPageRenderCacheBroker, PageRenderCacheBroker>();
        services.AddTransient<IPrivilegeBroker, PrivilegeBroker>();
        services.AddTransient<IRenderFileContentBroker, RenderFileContentBroker>();
        services.AddTransient<IResourceBroker, ResourceBroker>();
        services.AddTransient<IRoleBroker, RoleBroker>();
        services.AddTransient<IScriptBroker, ScriptBroker>();
        services.AddTransient<ISubmissionBroker, SubmissionBroker>();
        services.AddTransient<ITemplateBroker, TemplateBroker>();
        services.AddTransient<IAuthorizationBroker, AuthorizationBroker>();
        services.AddTransient<IJsonBroker, JsonBroker>();
        services.AddTransient<
            ITemplateContentBroker,
            TemplateContentBroker>();
        services.AddTransient<
            IWorkflowExecutionBroker,
            WorkflowExecutionBroker>();
        services.AddTransient<IUserRoleBroker, UserRoleBroker>();
    }

    private static void AddCoordinations(this IServiceCollection services)
    {
        services.AddTransient<
            IPageRenderCoordinationService,
            PageRenderCoordinationService>();
        services.AddTransient<IAppRenderableCoordinationService, AppRenderableCoordinationService>();
        services.AddTransient<IAppPageComponentCoordinationService, AppPageComponentCoordinationService>();
        services.AddTransient<IAppSupportingResourcesCoordinationService, AppSupportingResourcesCoordinationService>();
        services.AddTransient<IPageCoordinationService, PageCoordinationService>();
        services.AddTransient<IPageStructureCoordinationService, PageStructureCoordinationService>();
    }

    private static void AddEventHandlers(this IServiceCollection services)
    {
        services.AddTransient<IAppManager, AppManager>();
        services.AddTransient<IAuthorizationManager, AuthorizationManager>();
        services.AddTransient<IComponentManager, ComponentManager>();
        services.AddTransient<IContentManagementPackageManager, ContentManagementPackageManager>();
        services.AddTransient<IComponentRenderer, ComponentRenderer>();
        services.AddTransient<IPageManager, PageManager>();
        services.AddTransient<IPageRenderCacheManager, PageRenderCacheManager>();
        services.AddTransient<IPageRenderer, PageRenderer>();
        services.AddTransient<ITemplateManager, TemplateManager>();
        services.AddTransient<ITemplateRenderer, TemplateRenderer>();
        services.AddTransient<IContentManagementEventHandlers, ContentManagementEventHandlers>();
        services.AddTransient<IPageRenderCacheEventHandlers, PageRenderCacheEventHandlers>();
        services.AddTransient<
            IPageRenderCacheMissEventHandler,
            PageRenderCacheMissEventHandler>();
    }

    private static void AddRendering(this IServiceCollection services)
    {
        services.AddTransient<
            ICachedPageRenderOrchestrationService,
            CachedPageRenderOrchestrationService>();

        services.AddTransient<
            IUncachedPageRenderOrchestrationService,
            UncachedPageRenderOrchestrationService>();
        services.AddTransient<IPageRenderAggregationService, PageRenderAggregationService>();
        services.AddTransient<
            IPageRenderCacheBuildAggregationService,
            PageRenderCacheBuildAggregationService>();
        services.AddTransient<IPageRenderOrchestrationService, PageRenderOrchestrationService>();
        services.AddTransient<IPageRenderProcessingService, PageRenderProcessingService>();
        services.AddTransient<IPageRenderExecutionOrchestrationService, PageRenderExecutionOrchestrationService>();
        services.AddTransient<IMetadataCacheService, MetadataCacheService>();
        services.AddTransient<ICommonObjectCacheService, CommonObjectCacheService>();
        services.AddTransient<IMarkupRenderService, MarkupRenderService>();
        services.AddScoped<IRenderBroker, RenderBroker>();
        services.AddScoped<
            ICultureLinkTagHandlingProcessingService,
            CultureLinkTagHandlingProcessingService>();
        services.AddScoped<ITagHandlingProcessingService>(
            implementationFactory: serviceProvider =>
                serviceProvider.GetRequiredService<
                    ICultureLinkTagHandlingProcessingService>());
        services.AddScoped<
            IMetadataTagHandlingProcessingService,
            MetadataTagHandlingProcessingService>();
        services.AddScoped<ITagHandlingProcessingService>(
            implementationFactory: serviceProvider =>
                serviceProvider.GetRequiredService<
                    IMetadataTagHandlingProcessingService>());
        services.AddScoped<
            INavigationTagHandlingProcessingService,
            NavigationTagHandlingProcessingService>();
        services.AddScoped<ITagHandlingProcessingService>(
            implementationFactory: serviceProvider =>
                serviceProvider.GetRequiredService<
                    INavigationTagHandlingProcessingService>());
        services.AddScoped<
            IContentTagHandlingProcessingService,
            ContentTagHandlingProcessingService>();
        services.AddScoped<ITagHandlingProcessingService>(
            implementationFactory: serviceProvider =>
                serviceProvider.GetRequiredService<
                    IContentTagHandlingProcessingService>());
        services.AddScoped<
            IComponentTagHandlingProcessingService,
            ComponentTagHandlingProcessingService>();
        services.AddScoped<ITagHandlingProcessingService>(
            implementationFactory: serviceProvider =>
                serviceProvider.GetRequiredService<
                    IComponentTagHandlingProcessingService>());
        services.AddScoped<
            IScriptTagHandlingProcessingService,
            ScriptTagHandlingProcessingService>();
        services.AddScoped<ITagHandlingProcessingService>(
            implementationFactory: serviceProvider =>
                serviceProvider.GetRequiredService<
                    IScriptTagHandlingProcessingService>());
        services.AddScoped<
            IReplacementTagHandlingProcessingService,
            ReplacementTagHandlingProcessingService>();
        services.AddScoped<ITagHandlingProcessingService>(
            implementationFactory: serviceProvider =>
                serviceProvider.GetRequiredService<
                    IReplacementTagHandlingProcessingService>());
        services.AddScoped<
            IDmsTagHandlingProcessingService,
            DmsTagHandlingProcessingService>();
        services.AddScoped<ITagHandlingProcessingService>(
            implementationFactory: serviceProvider =>
                serviceProvider.GetRequiredService<
                    IDmsTagHandlingProcessingService>());
        services.AddScoped<
            IResourceTagHandlingProcessingService,
            ResourceTagHandlingProcessingService>();
        services.AddScoped<ITagHandlingProcessingService>(
            implementationFactory: serviceProvider =>
                serviceProvider.GetRequiredService<
                    IResourceTagHandlingProcessingService>());
        services.AddScoped<
            IExecuteTagHandlingProcessingService,
            ExecuteTagHandlingProcessingService>();
        services.AddScoped<ITagHandlingProcessingService>(
            implementationFactory: serviceProvider =>
                serviceProvider.GetRequiredService<
                    IExecuteTagHandlingProcessingService>());
        services.AddTransient<IComponentReaderBroker, ComponentReaderBroker>();
        services.AddTransient<IScriptReaderBroker, ScriptReaderBroker>();
        services.AddTransient<IMetadataReaderBroker, MetadataReaderBroker>();
        services.AddTransient<ICommonObjectReaderBroker, CommonObjectReaderBroker>();
    }

    private static void AddFoundations(this IServiceCollection services)
    {
        services.AddTransient<IHttpContextService, HttpContextService>();
        services.AddTransient<IPageAuthorizationService, PageAuthorizationService>();
        services.AddTransient<IEventHandlerService, EventHandlerService>();
        services.AddTransient<IJsonService, JsonService>();
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
        services.AddTransient<
            IPageRenderCacheMissEventService,
            PageRenderCacheMissEventService>();
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
        services.AddTransient<IPageRenderCacheService, PageRenderCacheService>();
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
        services.AddTransient<IComponentRenderService, ComponentRenderService>();
        services.AddTransient<IPageRenderService, PageRenderService>();
        services.AddTransient<ITemplateRenderService, TemplateRenderService>();
        services.AddTransient<IResourceProvider, CoreResourceBroker>();
        services.AddSingleton<ICommonObjectCache, CommonObjectCacheDependency>();
        services.AddSingleton<MetadataCacheDependency>();
        services.AddSingleton<IMetadataCache>(
            implementationFactory: serviceProvider =>
                serviceProvider.GetRequiredService<MetadataCacheDependency>());
    }

    private static void AddOrchestrations(this IServiceCollection services)
    {
        services.AddTransient<
            IPageContextOrchestrationService,
            PageContextOrchestrationService>();
        services.AddSingleton<PageRenderCacheImportState>();
        services.AddTransient<IContentManagementMigrationAggregationService, ContentManagementMigrationAggregationService>();
        services.AddTransient<IAppCultureOrchestrationService, AppCultureOrchestrationService>();
        services.AddTransient<IAppCultureManager, AppCultureOrchestrationService>();
        services.AddTransient<IAppOrchestrationService, AppOrchestrationService>();
        services.AddTransient<ICommonObjectOrchestrationService, CommonObjectOrchestrationService>();
        services.AddTransient<ICommonObjectManager, CommonObjectOrchestrationService>();
        services.AddTransient<IComponentOrchestrationService, ComponentOrchestrationService>();
        services.AddTransient<IComponentRenderOrchestrationService, ComponentRenderOrchestrationService>();
        services.AddTransient<IContentOrchestrationService, ContentOrchestrationService>();
        services.AddTransient<IContentManager, ContentOrchestrationService>();
        services.AddTransient<ICultureOrchestrationService, CultureOrchestrationService>();
        services.AddTransient<ICultureManager, CultureOrchestrationService>();
        services.AddTransient<ILayoutOrchestrationService, LayoutOrchestrationService>();
        services.AddTransient<ILayoutManager, LayoutOrchestrationService>();
        services.AddTransient<IMigrationSupportOrchestrationService, MigrationSupportOrchestrationService>();
        services.AddTransient<IPackageItemOrchestrationService, PackageItemOrchestrationService>();
        services.AddTransient<IPackageOrchestrationService, PackageOrchestrationService>();
        services.AddTransient<IPageInfoOrchestrationService, PageInfoOrchestrationService>();
        services.AddTransient<IPageInfoManager, PageInfoOrchestrationService>();
        services.AddTransient<IPageOrchestrationService, PageOrchestrationService>();
        services.AddTransient<IPageRenderCacheOrchestrationService, PageRenderCacheOrchestrationService>();
        services.AddKeyedTransient<IPageRenderCacheOrchestrationService>(
            serviceKey: "PageRenderCache",
            implementationFactory: (serviceProvider, _) =>
                serviceProvider.GetRequiredService<IPageRenderCacheOrchestrationService>());
        services.AddTransient<IPageRoleOrchestrationService, PageRoleOrchestrationService>();
        services.AddTransient<IPageRoleManager, PageRoleOrchestrationService>();
        services.AddTransient<
            IPageRoleImportOrchestrationService,
            PageRoleImportOrchestrationService>();
        services.AddTransient<IResourceOrchestrationService, ResourceOrchestrationService>();
        services.AddTransient<IResourceManager, ResourceOrchestrationService>();
        services.AddTransient<IScriptOrchestrationService, ScriptOrchestrationService>();
        services.AddTransient<IScriptManager, ScriptOrchestrationService>();
        services.AddTransient<ISubmissionOrchestrationService, SubmissionOrchestrationService>();
        services.AddTransient<ISubmissionManager, SubmissionOrchestrationService>();
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
        services.AddTransient<IAppUserProcessingService, AppUserProcessingService>();
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
        services.AddTransient<IJsonProcessingService, JsonProcessingService>();
        services.AddTransient<IPackageEventProcessingService, PackageEventProcessingService>();
        services.AddTransient<IPackageExportProcessingService, PackageExportProcessingService>();
        services.AddTransient<IPackageItemEventProcessingService, PackageItemEventProcessingService>();
        services.AddTransient<IPackageItemProcessingService, PackageItemProcessingService>();
        services.AddTransient<IPackageProcessingService, PackageProcessingService>();
        services.AddTransient<IPageEventProcessingService, PageEventProcessingService>();
        services.AddTransient<
            IPageRenderCacheMissEventProcessingService,
            PageRenderCacheMissEventProcessingService>();
        services.AddTransient<IPageInfoEventProcessingService, PageInfoEventProcessingService>();
        services.AddTransient<IPageInfoProcessingService, PageInfoProcessingService>();
        services.AddTransient<IPageProcessingService, PageProcessingService>();
        services.AddTransient<IPageRenderCacheProcessingService, PageRenderCacheProcessingService>();
        services.AddTransient<IPageRenderCacheQueryProcessingService, PageRenderCacheQueryProcessingService>();
        services.AddTransient<IPageRoleEventProcessingService, PageRoleEventProcessingService>();
        services.AddTransient<IPageRoleProcessingService, PageRoleProcessingService>();
        services.AddTransient<
            IPageRoleImportLookupProcessingService,
            PageRoleImportLookupProcessingService>();
        services.AddTransient<
            IPageRoleImportPersistenceProcessingService,
            PageRoleImportPersistenceProcessingService>();
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

    private static void RegisterConfiguration(
        this IServiceCollection services,
        ContentManagementConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(argument: configuration);
        services.AddSingleton(implementationInstance: configuration);

        if (!string.IsNullOrWhiteSpace(configuration.ConnectionString))
        {
            services.AddData(
                configuration: new cCoder.Data.Models.DataConfiguration
                {
                    ConnectionString = configuration.ConnectionString,
                    DebugInfo = configuration.DebugInfo,
                    LogSQL = configuration.LogSQL,
                });
        }

        services.AddEventProviders(eventProviders: configuration.EventProviders);
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

        services.AddAspNet();

        if (builder is null)
        {
            services.AddApiDocumentation(
                documentName: documentName,
                newContentManagementConfiguration: newContentManagementConfiguration,
                useFullSchemaIds: useFullSchemaIds);
        }

        IEdmModel routeModel = configureModel.BuildRouteModel();
        DefaultODataBatchHandler batchHandler = new();

        string rootPath = string.IsNullOrWhiteSpace(value: newContentManagementConfiguration.RootPath)
            ? $"Api/{documentName}"
            : newContentManagementConfiguration.RootPath;

        IMvcBuilder mvcBuilder = services.AddControllers();

        mvcBuilder.AddOData(setupAction: options =>
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
        this IServiceCollection services,
        string documentName,
        ContentManagementConfiguration newContentManagementConfiguration,
        bool useFullSchemaIds) =>
        services.AddSwaggerGen(setupAction: options =>
                                  {
                                      options.ResolveConflictingActions(resolver: apiDescriptions => apiDescriptions.First());
                                      options.AddSwaggerDocuments(
                                          documentName: documentName,
                                          newContentManagementConfiguration:
                                              newContentManagementConfiguration);

                                      options.DocInclusionPredicate(
predicate: (swaggerDocumentName, apiDescription) =>
                                              newContentManagementConfiguration
                                                  .ShouldIncludeInDocument(
                                                      swaggerDocumentName:
                                                          swaggerDocumentName,
                                                      relativePath:
                                                          apiDescription
                                                              .RelativePath,
                                                      documentName:
                                                          documentName));

                                      if (useFullSchemaIds)
                                      {
                                          options.CustomSchemaIds(schemaIdSelector: type => type.FullName?.Replace(oldChar: '+', newChar: '.') ?? type.Name);
                                      }
                                  });

    private static void AddAspNet(this IServiceCollection services)
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