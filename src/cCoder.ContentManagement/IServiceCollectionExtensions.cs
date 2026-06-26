using cCoder.ContentManagement.Api.OData;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Events;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Exposures.Caching;
using cCoder.ContentManagement.Exposures.EventHandlers;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Rendering.Services.Foundations;
using cCoder.ContentManagement.Rendering.Services.Orchestrations;
using cCoder.ContentManagement.Services;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Foundations;
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
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi;

namespace cCoder.ContentManagement;

public static partial class IServiceCollectionExtensions
{
    public static void AddContentManagementWeb(
        this IServiceCollection services,
        Action<ContentManagementConfiguration> configure = null,
        ODataConventionModelBuilder builder = null) =>
        services.AddConfiguredContentManagementWeb((_, configuration) => configure?.Invoke(configuration), builder);

    public static void AddContentManagementHostedServices(
        this IServiceCollection services,
        Action<ContentManagementConfiguration> configure = null) =>
        services.AddConfiguredContentManagement((_, configuration) => configure?.Invoke(configuration));

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

    private static void AddContentManagementWeb(this IServiceCollection services, ODataConventionModelBuilder builder = null)
    {
        services.AddContentManagement();

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
        services.AddTransient<IComponentRenderCoordinationService, ComponentRenderCoordinationService>();
        services.AddTransient<IPageCoordinationService, PageCoordinationService>();
        services.AddTransient<ITemplateRenderCoordinationService, TemplateRenderCoordinationService>();
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
        services.AddTransient<cCoder.ContentManagement.Services.ICurrentAppResolver, CurrentAppResolver>();
        services.AddTransient<IContentManagementMetadataTypeService, ContentManagementMetadataTypeService>();
        services.AddTransient<IRenderFileContentService, RenderFileContentService>();
        services.AddTransient<IResourceProvider, CoreResourceProvider>();
        services.AddSingleton<ICommonObjectCache, CommonObjectCache>();
        services.AddSingleton<MetadataCache>();
        services.AddSingleton<IMetadataCache>(serviceProvider => serviceProvider.GetRequiredService<MetadataCache>());
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
}
