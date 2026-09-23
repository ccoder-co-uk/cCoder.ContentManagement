// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Extensions.OData;
using cCoder.ContentManagement.Extensions;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Events;
using cCoder.ContentManagement.Brokers.Exports;
using cCoder.ContentManagement.Brokers.Authorizations;
using cCoder.ContentManagement.Brokers.HttpContexts;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Brokers.ServiceProviders;
using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Exposures.Caching;
using cCoder.ContentManagement.Dependencies;
using cCoder.ContentManagement.Brokers.Rendering;
using cCoder.ContentManagement.Brokers.Caching;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Rendering.Services.Foundations;
using cCoder.ContentManagement.Rendering.Services.Processings;
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
using cCoder.ContentManagement.Services.Foundations.TemplateContents;
using cCoder.Data;
using cCoder.ContentManagement.Services.Foundations.Serialization;
using cCoder.ContentManagement.Services.Foundations.Rendering;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Orchestrations.Caching;
using cCoder.ContentManagement.Services.Orchestrations.PageContexts;
using cCoder.ContentManagement.Services.Processings;
using cCoder.ContentManagement.Services.Processings.HttpContexts;
using cCoder.ContentManagement.Services.Processings.PageContexts;
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
        services.AddExposures();
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
        services.AddExposures();
        services.AddRendering();
    }

    private static void AddServiceProviderDependencies(
        this IServiceCollection services)
    {
        services.AddTransient<IServiceProviderBroker, ServiceProviderBroker>();

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

    }

    private static void AddEventingTypes(this IServiceCollection services)
    {
        services.AddEventingForType<App>();
        services.AddEventingForType<AppCulture>();
        services.AddEventingForType<CommonObject>();
        services.AddEventingForType<CommonObject[]>();
        services.AddEventingForType<Component>();
        services.AddEventingForType<Content>();
        services.AddEventingForType<Culture>();
        services.AddEventingForType<Layout>();
        services.AddEventingForType<Package>();
        services.AddEventingForType<PackageImportEvent>();
        services.AddEventingForType<PackageItemImportEvent<Component>>();
        services.AddEventingForType<PackageItemImportEvent<Layout>>();
        services.AddEventingForType<PackageItemImportEvent<Page>>();
        services.AddEventingForType<PackageItemImportEvent<Resource>>();
        services.AddEventingForType<PackageItemImportEvent<Script>>();
        services.AddEventingForType<PackageItemImportEvent<Template>>();
        services.AddEventingForType<PackageItemImportEvent<CommonObject>>();
        services.AddEventingForType<PackageItem>();
        services.AddEventingForType<Page>();
        services.AddEventingForType<PageInfo>();
        services.AddEventingForType<PageRole>();
        services.AddEventingForType<Resource>();
        services.AddEventingForType<Script>();
        services.AddEventingForType<Submission>();
        services.AddEventingForType<Template>();
        services.AddEventingForType<HttpPageRenderOperation>();
        services.AddEventingForType<TagHandlingOperation>();
    }

    private static void AddDependencies(this IServiceCollection services)
    {
        services.AddTransient<WorkflowExecutionDependency>();
    }

    private static void AddBrokers(this IServiceCollection services)
    {
        services.AddSingleton<MemoryCacheDependency>();
        services.AddSingleton<ICacheBroker, CacheBroker>();
        services.AddTransient<IMetadataTypeCacheBroker, MetadataTypeCacheBroker>();
        services.AddTransient<IContentRenderBroker, ContentRenderBroker>();
        services.AddTransient<Brokers.Loggings.ILoggingBroker, Brokers.Loggings.LoggingBroker>();
        services.AddHttpContextAccessor();

        services.AddTransient<IHttpContextBroker>(
            implementationFactory: serviceProvider =>
                new HttpContextBroker(
                    httpContext: serviceProvider
                        .GetRequiredService<IHttpContextAccessor>()
                        .HttpContext));
        services.AddTransient<IPageAuthorizationBroker, PageAuthorizationBroker>();
        services.AddTransient<IAppCultureEventBroker, AppCultureEventBroker>();
        services.AddTransient<IAppEventBroker, AppEventBroker>();
        services.AddTransient<ICommonObjectEventBroker, CommonObjectEventBroker>();
        services.AddTransient<IComponentEventBroker, ComponentEventBroker>();
        services.AddTransient<IContentEventBroker, ContentEventBroker>();
        services.AddTransient<ICultureEventBroker, CultureEventBroker>();
        services.AddTransient<ILayoutEventBroker, LayoutEventBroker>();
        services.AddTransient<IPackageItemEventBroker, PackageItemEventBroker>();
        services.AddTransient<IPackageImportEventBroker, PackageImportEventBroker>();
        services.AddTransient<IPageEventBroker, PageEventBroker>();
        services.AddTransient<IPageInfoEventBroker, PageInfoEventBroker>();
        services.AddTransient<IPageRoleEventBroker, PageRoleEventBroker>();
        services.AddTransient<IResourceEventBroker, ResourceEventBroker>();
        services.AddTransient<IScriptEventBroker, ScriptEventBroker>();
        services.AddTransient<ISubmissionEventBroker, SubmissionEventBroker>();
        services.AddTransient<ITemplateEventBroker, TemplateEventBroker>();
        services.AddTransient<IRenderEventBroker, RenderEventBroker>();
        services.AddTransient<IAppBroker, AppBroker>();
        services.AddTransient<IAppCultureBroker, AppCultureBroker>();
        services.AddTransient<ICommonObjectBroker, CommonObjectBroker>();
        services.AddTransient<IComponentBroker, ComponentBroker>();
        services.AddTransient<IContentBroker, ContentBroker>();
        services.AddTransient<ICultureBroker, CultureBroker>();
        services.AddTransient<ILayoutBroker, LayoutBroker>();
        services.AddTransient<IPageBroker, PageBroker>();
        services.AddTransient<IPageRenderDataBroker, PageRenderDataBroker>();
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
        services.AddTransient<IFingerprintBroker, FingerprintBroker>();
        services.AddTransient<IJsonBroker, JsonBroker>();
        services.AddTransient<IMetadataTypeBroker, MetadataTypeBroker>();
        services.AddTransient<IRegularExpressionBroker, RegularExpressionBroker>();
        services.AddTransient<IPackageExportBroker, PackageExportBroker>();
        services.AddTransient<ITemplateStreamBroker, TemplateStreamBroker>();
        services.AddTransient<IHtmlToPdfBroker, HtmlToPdfBroker>();
        services.AddTransient<
            IWorkflowExecutionBroker,
            WorkflowExecutionBroker>();
        services.AddTransient<IUserRoleBroker, UserRoleBroker>();
    }

    private static void AddCoordinations(this IServiceCollection services)
    {
        services.AddTransient<IAppManagerAggregationService, AppManagerAggregationService>();
        services.AddTransient<ITemplateManagerAggregationService, TemplateManagerAggregationService>();
        services.AddTransient<IAppRenderableCoordinationService, AppRenderableCoordinationService>();
        services.AddTransient<IAppPageComponentCoordinationService, AppPageComponentCoordinationService>();
        services.AddTransient<IAppSupportingResourcesCoordinationService, AppSupportingResourcesCoordinationService>();
        services.AddTransient<IAppLifecycleCoordinationService, AppLifecycleCoordinationService>();
        services.AddTransient<IAppManagerCoordinationService, AppManagerCoordinationService>();
        services.AddTransient<ICommonObjectCoordinationService, CommonObjectCoordinationService>();
        services.AddTransient<
            IContentManagementPackageCoordinationService,
            ContentManagementPackageCoordinationService>();
        services.AddTransient<IPageCoordinationService, PageCoordinationService>();
        services.AddTransient<IPageImportOrchestrationService, PageImportOrchestrationService>();
        services.AddTransient<
            IPagePackageImportCoordinationService,
            PagePackageImportCoordinationService>();
        services.AddTransient<IPageStructureCoordinationService, PageStructureCoordinationService>();
    }

    private static void AddExposures(this IServiceCollection services)
    {
        services.AddTransient<IAppManager, AppManager>();
        services.AddTransient<IComponentManager, ComponentManager>();
        services.AddTransient<IContentManagementPackageManager, ContentManagementPackageManager>();
        services.AddTransient<IComponentRenderer, ComponentRenderer>();
        services.AddTransient<IPageManager, PageManager>();
        services.AddTransient<IPageRenderCacheManager, PageRenderCacheManager>();
        services.AddTransient<IPageRenderer, PageRenderer>();
        services.AddTransient<ITemplateManager, TemplateManager>();
        services.AddTransient<ITemplateRenderer, TemplateRenderer>();
    }

    private static void AddRendering(this IServiceCollection services)
    {
        services.AddTransient<IRenderAggregationService, RenderAggregationService>();
        services.AddTransient<
            ICachedPageRenderOrchestrationService,
            CachedPageRenderOrchestrationService>();
        services.AddTransient<IRenderer>(
            implementationFactory: serviceProvider =>
                serviceProvider.GetRequiredService<IRenderAggregationService>());
        services.AddTransient<
            IUncachedPageRenderOrchestrationService,
            UncachedPageRenderOrchestrationService>();
        services.AddTransient<
            IPageRenderCacheAggregationService,
            PageRenderCacheAggregationService>();
        services.AddTransient<IPageRenderOrchestrationService, PageRenderOrchestrationService>();
        services.AddTransient<IPageRenderProcessingService, PageRenderProcessingService>();
        services.AddTransient<IMetadataCacheService, MetadataCacheService>();
        services.AddTransient<IMetadataCacheSourceService, MetadataCacheSourceService>();
        services.AddTransient<IMetadataRenderCacheService, MetadataRenderCacheService>();
        services.AddTransient<ICommonObjectCacheService, CommonObjectCacheService>();
        services.AddTransient<
            ICommonObjectRenderCacheService,
            CommonObjectRenderCacheService>();
        services.AddTransient<ICommonObjectLatestCacheService, CommonObjectLatestCacheService>();
        services.AddTransient<IMarkupRenderService, MarkupRenderService>();
        services.AddTransient<ICachedPageRenderService, CachedPageRenderService>();
        services.AddTransient<IMarkupRenderProcessingService, MarkupRenderProcessingService>();
        services.AddTransient<
            IMarkupRenderTagHandlingProcessingService,
            MarkupRenderTagHandlingProcessingService>();
        services.AddTransient<IMetadataCacheProcessingService, MetadataCacheProcessingService>();
        services.AddTransient<
            IMetadataCacheSourceProcessingService,
            MetadataCacheSourceProcessingService>();
        services.AddTransient<
            IMetadataRenderCacheProcessingService,
            MetadataRenderCacheProcessingService>();
        services.AddTransient<ICommonObjectCacheProcessingService, CommonObjectCacheProcessingService>();
        services.AddTransient<
            ICommonObjectRenderCacheProcessingService,
            CommonObjectRenderCacheProcessingService>();
        services.AddTransient<ICommonObjectLatestCacheProcessingService, CommonObjectLatestCacheProcessingService>();
        services.AddTransient<IComponentReaderBroker, ComponentReaderBroker>();
        services.AddTransient<IScriptReaderBroker, ScriptReaderBroker>();
    }

    private static void AddFoundations(this IServiceCollection services)
    {
        services.AddTransient<IHttpContextService, HttpContextService>();
        services.AddTransient<IPageAuthorizationService, PageAuthorizationService>();
        services.AddTransient<IJsonService, JsonService>();
        services.AddTransient<IHtmlRenderService, HtmlRenderService>();
        services.AddTransient<IAuthorizationService, AuthorizationService>();
        services.AddTransient<IAppCultureEventService, AppCultureEventService>();
        services.AddTransient<IAppEventService, AppEventService>();
        services.AddTransient<ICommonObjectEventService, CommonObjectEventService>();
        services.AddTransient<IComponentEventService, ComponentEventService>();
        services.AddTransient<IContentEventService, ContentEventService>();
        services.AddTransient<ICultureEventService, CultureEventService>();
        services.AddTransient<ILayoutEventService, LayoutEventService>();
        services.AddTransient<IPackageItemEventService, PackageItemEventService>();
        services.AddTransient<IPackageImportEventService, PackageImportEventService>();
        services.AddTransient<IPageEventService, PageEventService>();
        services.AddTransient<IPageInfoEventService, PageInfoEventService>();
        services.AddTransient<IPageRoleEventService, PageRoleEventService>();
        services.AddTransient<IResourceEventService, ResourceEventService>();
        services.AddTransient<IScriptEventService, ScriptEventService>();
        services.AddTransient<ISubmissionEventService, SubmissionEventService>();
        services.AddTransient<ITemplateEventService, TemplateEventService>();
        services.AddTransient<
            IHttpPageRenderOperationEventService,
            HttpPageRenderOperationEventService>();
        services.AddTransient<
            ITagHandlingOperationEventService,
            TagHandlingOperationEventService>();
        services.AddTransient<IPackageExportService, PackageExportService>();
        services.AddTransient<IAppCultureService, AppCultureService>();
        services.AddTransient<IAppService, AppService>();
        services.AddTransient<IPrivilegeService, PrivilegeService>();
        services.AddTransient<IRoleService, RoleService>();
        services.AddTransient<IUserRoleService, UserRoleService>();
        services.AddTransient<ICommonObjectService, CommonObjectService>();
        services.AddTransient<IComponentService, ComponentService>();
        services.AddTransient<IContentService, ContentService>();
        services.AddTransient<ICultureService, CultureService>();
        services.AddTransient<ILayoutService, LayoutService>();
        services.AddTransient<IPageInfoService, PageInfoService>();
        services.AddTransient<IPageRoleService, PageRoleService>();
        services.AddTransient<IPageService, PageService>();
        services.AddTransient<IPageRenderDataService, PageRenderDataService>();
        services.AddTransient<IPageRenderCacheService, PageRenderCacheService>();
        services.AddTransient<IResourceService, ResourceService>();
        services.AddTransient<IScriptService, ScriptService>();
        services.AddTransient<ISubmissionService, SubmissionService>();
        services.AddTransient<ITemplateService, TemplateService>();
        services.AddTransient<ITemplateContentService, TemplateContentService>();
        services.AddTransient<IHtmlToPdfService, HtmlToPdfService>();

        services.AddTransient<ICurrentAppResolver, CurrentAppManager>();
        services.AddTransient<IContentManagementMetadataTypeService, ContentManagementMetadataTypeService>();
        services.AddTransient<IRenderFileContentService, RenderFileContentService>();
        services.AddTransient<IComponentRenderService, ComponentRenderService>();
        services.AddTransient<ITemplateRenderService, TemplateRenderService>();
        services.AddTransient<IPageRenderService, PageRenderService>();
        services.AddTransient<IResourceProvider, CoreResourceBroker>();
        services.AddScoped<ICommonObjectCache, CommonObjectCacheManager>();
        services.AddSingleton<IMetadataCache, MetadataCacheManager>();
    }

    private static void AddOrchestrations(this IServiceCollection services)
    {
        services.AddTransient<
            ICommonObjectCacheOrchestrationService,
            CommonObjectCacheOrchestrationService>();
        services.AddTransient<
            IMetadataCacheOrchestrationService,
            MetadataCacheOrchestrationService>();
        services.AddTransient<
            IPageContextOrchestrationService,
            PageContextOrchestrationService>();
        services.AddSingleton<PageRenderCacheImportState>();
        services.AddTransient<
            IContentManagementPackageImportOrchestrationService,
            ContentManagementPackageImportOrchestrationService>();
        services.AddTransient<
            IContentManagementPackageExportOrchestrationService,
            ContentManagementPackageExportOrchestrationService>();
        services.AddTransient<IAppCultureOrchestrationService, AppCultureOrchestrationService>();
        services.AddTransient<IAppCultureManager, AppCultureManager>();
        services.AddTransient<IAppOrchestrationService, AppOrchestrationService>();
        services.AddTransient<IAppBootstrapOrchestrationService, AppBootstrapOrchestrationService>();
        services.AddTransient<IAppRoleOrchestrationService, AppRoleOrchestrationService>();
        services.AddTransient<IAppPageOrderOrchestrationService, AppPageOrderOrchestrationService>();
        services.AddTransient<ICurrentAppOrchestrationService, CurrentAppOrchestrationService>();
        services.AddTransient<ICommonObjectOrchestrationService, CommonObjectOrchestrationService>();
        services.AddTransient<ICommonObjectEventOrchestrationService, CommonObjectEventOrchestrationService>();
        services.AddTransient<ICommonObjectManager, CommonObjectManager>();
        services.AddTransient<IComponentOrchestrationService, ComponentOrchestrationService>();
        services.AddTransient<IComponentRenderOrchestrationService, ComponentRenderOrchestrationService>();
        services.AddTransient<IContentOrchestrationService, ContentOrchestrationService>();
        services.AddTransient<IContentManager, ContentManager>();
        services.AddTransient<ICultureOrchestrationService, CultureOrchestrationService>();
        services.AddTransient<ICultureManager, CultureManager>();
        services.AddTransient<ILayoutOrchestrationService, LayoutOrchestrationService>();
        services.AddTransient<ILayoutManager, LayoutManager>();
        services.AddTransient<
            IRenderDataOrchestrationService,
            RenderDataOrchestrationService>();
        services.AddTransient<IPageInfoOrchestrationService, PageInfoOrchestrationService>();
        services.AddTransient<IPageInfoManager, PageInfoManager>();
        services.AddTransient<IPageOrchestrationService, PageOrchestrationService>();
        services.AddTransient<IPageRenderCacheOrchestrationService, PageRenderCacheOrchestrationService>();
        services.AddTransient<IPageRoleOrchestrationService, PageRoleOrchestrationService>();
        services.AddTransient<IPageRoleManager, PageRoleManager>();
        services.AddTransient<
            IPageRoleImportOrchestrationService,
            PageRoleImportOrchestrationService>();
        services.AddTransient<IResourceOrchestrationService, ResourceOrchestrationService>();
        services.AddTransient<IResourceManager, ResourceManager>();
        services.AddTransient<IScriptOrchestrationService, ScriptOrchestrationService>();
        services.AddTransient<IScriptManager, ScriptManager>();
        services.AddTransient<ISubmissionOrchestrationService, SubmissionOrchestrationService>();
        services.AddTransient<ISubmissionManager, SubmissionManager>();
        services.AddTransient<ITemplateOrchestrationService, TemplateOrchestrationService>();
        services.AddTransient<ITemplateContentOrchestrationService, TemplateContentOrchestrationService>();
        services.AddTransient<ITemplateRenderOrchestrationService, TemplateRenderOrchestrationService>();
        services.AddTransient<IRenderEventOrchestrationService, RenderEventOrchestrationService>();
        services.AddTransient<IMarkupRenderOrchestrationService, MarkupRenderOrchestrationService>();
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
        services.AddTransient<IJsonProcessingService, JsonProcessingService>();
        services.AddTransient<IHtmlRenderProcessingService, HtmlRenderProcessingService>();
        services.AddTransient<IPackageExportProcessingService, PackageExportProcessingService>();
        services.AddTransient<IPackageItemEventProcessingService, PackageItemEventProcessingService>();
        services.AddTransient<
            IPackageImportEventProcessingService,
            PackageImportEventProcessingService>();
        services.AddTransient<IPageEventProcessingService, PageEventProcessingService>();
        services.AddTransient<IPageInfoEventProcessingService, PageInfoEventProcessingService>();
        services.AddTransient<IPageInfoProcessingService, PageInfoProcessingService>();
        services.AddTransient<IPageProcessingService, PageProcessingService>();
        services.AddTransient<
            IPageRenderDataProcessingService,
            PageRenderDataProcessingService>();
        services.AddTransient<
            IHttpContextProcessingService,
            HttpContextProcessingService>();
        services.AddTransient<
            IPageAuthorizationProcessingService,
            PageAuthorizationProcessingService>();
        services.AddTransient<IPageRenderCacheProcessingService, PageRenderCacheProcessingService>();
        services.AddTransient<IPageRenderCacheQueryProcessingService, PageRenderCacheQueryProcessingService>();
        services.AddTransient<
            ICachedPageRenderProcessingService,
            CachedPageRenderProcessingService>();
        services.AddTransient<IPageRoleEventProcessingService, PageRoleEventProcessingService>();
        services.AddTransient<IPageRoleProcessingService, PageRoleProcessingService>();
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
        services.AddTransient<ITemplateContentProcessingService, TemplateContentProcessingService>();
        services.AddTransient<IHtmlToPdfProcessingService, HtmlToPdfProcessingService>();
        services.AddTransient<ITemplateRenderProcessingService, TemplateRenderProcessingService>();
        services.AddTransient<
            IHttpPageRenderOperationEventProcessingService,
            HttpPageRenderOperationEventProcessingService>();
        services.AddTransient<
            ITagHandlingOperationEventProcessingService,
            TagHandlingOperationEventProcessingService>();
    }

    private static void RegisterConfiguration(
        this IServiceCollection services,
        ContentManagementConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(argument: configuration);
        services.AddSingleton(implementationInstance: configuration);

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
                                          documentName: documentName);

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