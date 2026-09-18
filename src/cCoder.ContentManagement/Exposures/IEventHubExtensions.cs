// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures.Caching;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Foundations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Eventing;
using cCoder.ContentManagement.Rendering.Services.Processings;

namespace cCoder.ContentManagement;

public static partial class IEventHubExtensions
{
    public static IEventHub ListenToContentManagementEvents(
        this IEventHub eventHub)
    {
        ListenToContentManagementBusinessEvents(eventHub: eventHub);

        return eventHub;
    }

    public static IEventHub ListenToFinalContentManagementEvents(
        this IEventHub eventHub)
    {
        ListenToFinalAppDeleteEvent(eventHub: eventHub);

        return eventHub;
    }

    public static IEventHub ListenToContentManagementWebEvents(
        this IEventHub eventHub)
    {
        ListenToContentManagementBusinessEvents(eventHub: eventHub);
        ListenToAppOwnedRenderCacheEvents(eventHub: eventHub);
        ListenToPageOwnedRenderCacheEvents(eventHub: eventHub);
        ListenToCommonObjectRenderCacheEvents(eventHub: eventHub);
        ListenToPackageImportRenderCacheEvents(eventHub: eventHub);
        ListenToRenderRequestEvents(eventHub: eventHub);
        ListenToRenderTagEvents(eventHub: eventHub);
        ListenToFinalAppDeleteEvent(eventHub: eventHub);

        return eventHub;
    }

    private static void ListenToContentManagementBusinessEvents(
        IEventHub eventHub)
    {
        ListenToAppSupportingResourcesEvents(eventHub: eventHub);
        ListenToAppRenderableEvents(eventHub: eventHub);
        ListenToAppPageComponentEvents(eventHub: eventHub);
        ListenToPageCoordinationEvents(eventHub: eventHub);
        ListenToPageStructureEvents(eventHub: eventHub);
        ListenToComponentPackageImportEvents(eventHub: eventHub);
        ListenToLayoutPackageImportEvents(eventHub: eventHub);
        ListenToPagePackageImportEvents(eventHub: eventHub);
        ListenToResourcePackageImportEvents(eventHub: eventHub);
        ListenToScriptPackageImportEvents(eventHub: eventHub);
        ListenToTemplatePackageImportEvents(eventHub: eventHub);
        ListenToCommonObjectPackageImportEvents(eventHub: eventHub);
    }

    private static void ListenToAppSupportingResourcesEvents(IEventHub eventHub)
    {
        eventHub.ListenToEvent(
            name: "app_add",
            handler: (IAppSupportingResourcesCoordinationService service, App app) =>
                service.HandleAppAddAsync(app: app));

        eventHub.ListenToEvent(
            name: "app_update",
            handler: (IAppSupportingResourcesCoordinationService service, App app) =>
                service.HandleAppUpdateAsync(app: app));

        eventHub.ListenToEvent(
            name: "app_delete",
            handler: (IAppSupportingResourcesCoordinationService service, App app) =>
                service.HandleAppDeleteAsync(app: app));
    }

    private static void ListenToAppRenderableEvents(IEventHub eventHub)
    {
        eventHub.ListenToEvent(
            name: "app_add",
            handler: (IAppRenderableCoordinationService service, App app) =>
                service.HandleAppAddAsync(app: app));

        eventHub.ListenToEvent(
            name: "app_update",
            handler: (IAppRenderableCoordinationService service, App app) =>
                service.HandleAppUpdateAsync(app: app));

        eventHub.ListenToEvent(
            name: "app_delete",
            handler: (IAppRenderableCoordinationService service, App app) =>
                service.HandleAppDeleteAsync(app: app));
    }

    private static void ListenToAppPageComponentEvents(IEventHub eventHub)
    {
        eventHub.ListenToEvent(
            name: "app_add",
            handler: (IAppPageComponentCoordinationService service, App app) =>
                service.HandleAppAddAsync(app: app));

        eventHub.ListenToEvent(
            name: "app_update",
            handler: (IAppPageComponentCoordinationService service, App app) =>
                service.HandleAppUpdateAsync(app: app));

        eventHub.ListenToEvent(
            name: "app_delete",
            handler: (IAppPageComponentCoordinationService service, App app) =>
                service.HandleAppDeleteAsync(app: app));
    }

    private static void ListenToPageCoordinationEvents(IEventHub eventHub)
    {
        eventHub.ListenToEvent(
            name: "page_add",
            handler: (IPageCoordinationService service, Page page) =>
                service.HandlePageAddAsync(page: page));

        eventHub.ListenToEvent(
            name: "page_update",
            handler: (IPageCoordinationService service, Page page) =>
                service.HandlePageUpdateAsync(page: page));

        eventHub.ListenToEvent(
            name: "page_delete",
            handler: (IPageCoordinationService service, Page page) =>
                service.HandlePageDeleteAsync(page: page));
    }

    private static void ListenToPageStructureEvents(IEventHub eventHub)
    {
        eventHub.ListenToEvent(
            name: "page_add",
            handler: (IPageStructureCoordinationService service, Page page) =>
                service.HandlePageAddAsync(page: page));

        eventHub.ListenToEvent(
            name: "page_update",
            handler: (IPageStructureCoordinationService service, Page page) =>
                service.HandlePageUpdateAsync(page: page));

        eventHub.ListenToEvent(
            name: "page_delete",
            handler: (IPageStructureCoordinationService service, Page page) =>
                service.HandlePageDeleteAsync(page: page));
    }

    private static void ListenToComponentPackageImportEvents(IEventHub eventHub) =>
        eventHub.ListenToEvent(
            name: "component_import",
            handler: (IComponentOrchestrationService service,
                PackageItemImportEvent<Component> import) =>
                service.ImportComponentsAsync(
                    appId: import.AppId!.Value,
                    items: import.Items));

    private static void ListenToLayoutPackageImportEvents(IEventHub eventHub) =>
        eventHub.ListenToEvent(
            name: "layout_import",
            handler: (ILayoutOrchestrationService service,
                PackageItemImportEvent<Layout> import) =>
                service.ImportLayoutsAsync(
                    appId: import.AppId!.Value,
                    items: import.Items));

    private static void ListenToPagePackageImportEvents(IEventHub eventHub) =>
        eventHub.ListenToEvent(
            name: "page_import",
            handler: (IPagePackageImportCoordinationService service,
                PackageItemImportEvent<Page> import) =>
                service.ImportPagesAsync(
                    appId: import.AppId!.Value,
                    pages: import.Items));

    private static void ListenToResourcePackageImportEvents(IEventHub eventHub) =>
        eventHub.ListenToEvent(
            name: "resource_import",
            handler: (IResourceOrchestrationService service,
                PackageItemImportEvent<Resource> import) =>
                service.ImportResourcesAsync(
                    appId: import.AppId!.Value,
                    items: import.Items));

    private static void ListenToScriptPackageImportEvents(IEventHub eventHub) =>
        eventHub.ListenToEvent(
            name: "script_import",
            handler: (IScriptOrchestrationService service,
                PackageItemImportEvent<Script> import) =>
                service.ImportScriptsAsync(
                    appId: import.AppId!.Value,
                    items: import.Items));

    private static void ListenToTemplatePackageImportEvents(IEventHub eventHub) =>
        eventHub.ListenToEvent(
            name: "template_import",
            handler: (ITemplateOrchestrationService service,
                PackageItemImportEvent<Template> import) =>
                service.ImportTemplatesAsync(
                    appId: import.AppId!.Value,
                    items: import.Items));

    private static void ListenToCommonObjectPackageImportEvents(IEventHub eventHub) =>
        eventHub.ListenToEvent(
            name: "common_objects_import",
            handler: (ICommonObjectCoordinationService service,
                PackageItemImportEvent<CommonObject> import) =>
                ImportCommonObjectsAsync(
                    service: service,
                    items: import.Items));

    private static async ValueTask ImportCommonObjectsAsync(
        ICommonObjectCoordinationService service,
        CommonObject[] items) =>
        _ = await service.AddAllCommonObjectsAsync(
            newCommonObjects: items);

    private static void ListenToAppOwnedRenderCacheEvents(IEventHub eventHub)
    {
        eventHub.ListenToEvent(
            name: "app_update",
            handler: (IPageRenderCacheAggregationService service, App app) =>
                service.DeleteAppAsync(appId: app.Id, fromEvent: true));

        eventHub.ListenToEvent(
            name: "app_delete",
            handler: (IPageRenderCacheAggregationService service, App app) =>
                service.DeleteAppAsync(appId: app.Id, fromEvent: true));

        eventHub.ListenToEvent<AppCulture, IPageRenderCacheAggregationService>(
            name: "app_culture_add",
            handler: static (service, appCulture) =>
                service.DeleteAppAsync(appId: appCulture.AppId, fromEvent: true));

        eventHub.ListenToEvent<AppCulture, IPageRenderCacheAggregationService>(
            name: "app_culture_delete",
            handler: static (service, appCulture) =>
                service.DeleteAppAsync(appId: appCulture.AppId, fromEvent: true));

        eventHub.ListenToEvent<Layout, IPageRenderCacheAggregationService>(
            name: "layout_add",
            handler: static (service, layout) =>
                service.DeleteAppAsync(appId: layout.AppId, fromEvent: true));

        eventHub.ListenToEvent<Layout, IPageRenderCacheAggregationService>(
            name: "layout_update",
            handler: static (service, layout) =>
                service.DeleteAppAsync(appId: layout.AppId, fromEvent: true));

        eventHub.ListenToEvent<Layout, IPageRenderCacheAggregationService>(
            name: "layout_delete",
            handler: static (service, layout) =>
                service.DeleteAppAsync(appId: layout.AppId, fromEvent: true));

        eventHub.ListenToEvent<Template, IPageRenderCacheAggregationService>(
            name: "template_add",
            handler: static (service, template) =>
                service.DeleteAppAsync(appId: template.AppId, fromEvent: true));

        eventHub.ListenToEvent<Template, IPageRenderCacheAggregationService>(
            name: "template_update",
            handler: static (service, template) =>
                service.DeleteAppAsync(appId: template.AppId, fromEvent: true));

        eventHub.ListenToEvent<Template, IPageRenderCacheAggregationService>(
            name: "template_delete",
            handler: static (service, template) =>
                service.DeleteAppAsync(appId: template.AppId, fromEvent: true));

        eventHub.ListenToEvent<Component, IPageRenderCacheAggregationService>(
            name: "component_add",
            handler: static (service, component) =>
                service.DeleteAppAsync(appId: component.AppId, fromEvent: true));

        eventHub.ListenToEvent<Component, IPageRenderCacheAggregationService>(
            name: "component_update",
            handler: static (service, component) =>
                service.DeleteAppAsync(appId: component.AppId, fromEvent: true));

        eventHub.ListenToEvent<Component, IPageRenderCacheAggregationService>(
            name: "component_delete",
            handler: static (service, component) =>
                service.DeleteAppAsync(appId: component.AppId, fromEvent: true));

        eventHub.ListenToEvent<Resource, IPageRenderCacheAggregationService>(
            name: "resource_add",
            handler: static (service, resource) =>
                service.DeleteAppAsync(appId: resource.AppId, fromEvent: true));

        eventHub.ListenToEvent<Resource, IPageRenderCacheAggregationService>(
            name: "resource_update",
            handler: static (service, resource) =>
                service.DeleteAppAsync(appId: resource.AppId, fromEvent: true));

        eventHub.ListenToEvent<Resource, IPageRenderCacheAggregationService>(
            name: "resource_delete",
            handler: static (service, resource) =>
                service.DeleteAppAsync(appId: resource.AppId, fromEvent: true));

        eventHub.ListenToEvent<Script, IPageRenderCacheAggregationService>(
            name: "script_add",
            handler: static (service, script) =>
                service.DeleteAppAsync(appId: script.AppId, fromEvent: true));

        eventHub.ListenToEvent<Script, IPageRenderCacheAggregationService>(
            name: "script_update",
            handler: static (service, script) =>
                service.DeleteAppAsync(appId: script.AppId, fromEvent: true));

        eventHub.ListenToEvent<Script, IPageRenderCacheAggregationService>(
            name: "script_delete",
            handler: static (service, script) =>
                service.DeleteAppAsync(appId: script.AppId, fromEvent: true));
    }

    private static void ListenToPageOwnedRenderCacheEvents(IEventHub eventHub)
    {
        eventHub.ListenToEvent(
            name: "page_add",
            handler: (IPageRenderCacheAggregationService service, Page page) =>
                service.DeletePageAsync(pageId: page.Id, fromEvent: true));

        eventHub.ListenToEvent(
            name: "page_update",
            handler: (IPageRenderCacheAggregationService service, Page page) =>
                service.DeletePageAsync(pageId: page.Id, fromEvent: true));

        eventHub.ListenToEvent(
            name: "page_delete",
            handler: (IPageRenderCacheAggregationService service, Page page) =>
                service.DeletePageAsync(pageId: page.Id, fromEvent: true));

        eventHub.ListenToEvent(
            name: "content_add",
            handler: (IPageRenderCacheAggregationService service, Content content) =>
                service.DeletePageAsync(pageId: content.PageId, fromEvent: true));

        eventHub.ListenToEvent(
            name: "content_update",
            handler: (IPageRenderCacheAggregationService service, Content content) =>
                service.DeletePageAsync(pageId: content.PageId, fromEvent: true));

        eventHub.ListenToEvent(
            name: "content_delete",
            handler: (IPageRenderCacheAggregationService service, Content content) =>
                service.DeletePageAsync(pageId: content.PageId, fromEvent: true));

        eventHub.ListenToEvent(
            name: "page_info_add",
            handler: (IPageRenderCacheAggregationService service, PageInfo pageInfo) =>
                service.DeletePageAsync(pageId: pageInfo.PageId, fromEvent: true));

        eventHub.ListenToEvent(
            name: "page_info_update",
            handler: (IPageRenderCacheAggregationService service, PageInfo pageInfo) =>
                service.DeletePageAsync(pageId: pageInfo.PageId, fromEvent: true));

        eventHub.ListenToEvent(
            name: "page_info_delete",
            handler: (IPageRenderCacheAggregationService service, PageInfo pageInfo) =>
                service.DeletePageAsync(pageId: pageInfo.PageId, fromEvent: true));
    }

    private static void ListenToCommonObjectRenderCacheEvents(IEventHub eventHub)
    {
        eventHub.ListenToEvent(
            name: "common_object_add",
            handler: (IPageRenderCacheAggregationService service, CommonObject commonObject) =>
                service.InvalidateCommonObjectConsumersAsync(
                    commonObjectType: commonObject.Type,
                    fromEvent: true));

        eventHub.ListenToEvent(
            name: "common_object_update",
            handler: (IPageRenderCacheAggregationService service, CommonObject commonObject) =>
                service.InvalidateCommonObjectConsumersAsync(
                    commonObjectType: commonObject.Type,
                    fromEvent: true));

        eventHub.ListenToEvent(
            name: "common_object_delete",
            handler: (IPageRenderCacheAggregationService service, CommonObject commonObject) =>
                service.InvalidateCommonObjectConsumersAsync(
                    commonObjectType: commonObject.Type,
                    fromEvent: true));

        eventHub.ListenToEvent(
            name: "common_objects_imported",
            handler: (IPageRenderCacheAggregationService service, CommonObject[] commonObjects) =>
                service.InvalidateCommonCacheAsync(fromEvent: true));
    }

    private static void ListenToPackageImportRenderCacheEvents(IEventHub eventHub) =>
        eventHub.ListenToEvent(
            name: "package_import_complete",
            handler: (IPageRenderCacheAggregationService service, PackageImportEvent args) =>
                service.InvalidatePackageAsync(appId: args.AppId));

    private static void ListenToRenderTagEvents(IEventHub eventHub)
    {
        eventHub.ListenToEvent<TagHandlingOperation, IMarkupRenderTagHandlingProcessingService>(
            name: "render_tags",
            handler: static (service, operation) =>
                service.RenderCultureLinkTagHandlingOperationAsync(
                    tagHandlingOperation: operation));

        eventHub.ListenToEvent<TagHandlingOperation, IMarkupRenderTagHandlingProcessingService>(
            name: "render_tags",
            handler: static (service, operation) =>
                service.RenderMetadataTagHandlingOperationAsync(
                    tagHandlingOperation: operation));

        eventHub.ListenToEvent<TagHandlingOperation, IMarkupRenderTagHandlingProcessingService>(
            name: "render_tags",
            handler: static (service, operation) =>
                service.RenderNavigationTagHandlingOperationAsync(
                    tagHandlingOperation: operation));

        eventHub.ListenToEvent<TagHandlingOperation, IMarkupRenderTagHandlingProcessingService>(
            name: "render_tags",
            handler: static (service, operation) =>
                service.RenderContentTagHandlingOperationAsync(
                    tagHandlingOperation: operation));

        eventHub.ListenToEvent<TagHandlingOperation, IMarkupRenderTagHandlingProcessingService>(
            name: "render_tags",
            handler: static (service, operation) =>
                service.RenderComponentTagHandlingOperationAsync(
                    tagHandlingOperation: operation));

        eventHub.ListenToEvent<TagHandlingOperation, IMarkupRenderTagHandlingProcessingService>(
            name: "render_tags",
            handler: static (service, operation) =>
                service.RenderScriptTagHandlingOperationAsync(
                    tagHandlingOperation: operation));

        eventHub.ListenToEvent<TagHandlingOperation, IMarkupRenderTagHandlingProcessingService>(
            name: "render_tags",
            handler: static (service, operation) =>
                service.RenderStyleTagHandlingOperationAsync(
                    tagHandlingOperation: operation));

        eventHub.ListenToEvent<TagHandlingOperation, IMarkupRenderTagHandlingProcessingService>(
            name: "render_tags",
            handler: static (service, operation) =>
                service.RenderReplacementTagHandlingOperationAsync(
                    tagHandlingOperation: operation));

        eventHub.ListenToEvent<TagHandlingOperation, IMarkupRenderTagHandlingProcessingService>(
            name: "render_tags",
            handler: static (service, operation) =>
                service.RenderDmsTagHandlingOperationAsync(
                    tagHandlingOperation: operation));

        eventHub.ListenToEvent<TagHandlingOperation, IMarkupRenderTagHandlingProcessingService>(
            name: "render_tags",
            handler: static (service, operation) =>
                service.RenderResourceTagHandlingOperationAsync(
                    tagHandlingOperation: operation));

        eventHub.ListenToEvent<TagHandlingOperation, IMarkupRenderTagHandlingProcessingService>(
            name: "render_tags",
            handler: static (service, operation) =>
                service.RenderExecuteTagHandlingOperationAsync(
                    tagHandlingOperation: operation));
    }

    private static void ListenToRenderRequestEvents(IEventHub eventHub)
    {
        eventHub.ListenToEvent<HttpPageRenderOperation, IUncachedPageRenderOrchestrationService>(
            name: "render_request",
            handler: static (service, operation) =>
                service.PrepareHttpPageRenderOperationAsync(
                    httpPageRenderOperation: operation));

        eventHub.ListenToEvent<HttpPageRenderOperation, IMarkupRenderOrchestrationService>(
            name: "render_request",
            handler: static (service, operation) =>
                service.RenderHttpPageRenderOperationAsync(
                    httpPageRenderOperation: operation));

        eventHub.ListenToEvent<HttpPageRenderOperation, IUncachedPageRenderOrchestrationService>(
            name: "render_request",
            handler: static (service, operation) =>
                service.CompleteHttpPageRenderOperationAsync(
                    httpPageRenderOperation: operation));
    }

    private static void ListenToFinalAppDeleteEvent(IEventHub eventHub) =>
        eventHub.ListenToEvent(
            name: "app_delete",
            handler: (IAppOrchestrationService service, App app) =>
                service.HandleAppDeleteAsync(app: app));
}