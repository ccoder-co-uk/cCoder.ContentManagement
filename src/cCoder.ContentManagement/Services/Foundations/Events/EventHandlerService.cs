// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models;
using cCoder.ContentManagement.Exposures.EventHandlers;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class EventHandlerService(IEventHubBroker eventHubBroker) : IEventHandlerService
{
    public void ListenToAllEvents() =>
        TryCatch(operation: () =>
    {
        ValidateListenToAllEvents(inputs: []);
        ValidateEventHubBroker(broker: eventHubBroker, parameterName: "eventHubBroker");
        ListenToAppEvents();
        ListenToPageEvents();
        ListenToPackageEvents();
        ListenToPageRenderCacheEvents();

    });

    public void ListenToFinalAppDeleteEvent() =>
        TryCatch(operation: () =>
        {
            ValidateListenToFinalAppDeleteEvent(inputs: []);

            ValidateEventHubBroker(
                broker: eventHubBroker,
                parameterName: "eventHubBroker");

            eventHubBroker.ListenToEvent(
                eventName: "app_delete",
                handler: (IAppOrchestrationService service, App app) =>
                    service.HandleAppDeleteAsync(app: app));
        });

    private void ListenToAppEvents()
    {
        ListenToAppAddEvents();
        ListenToAppUpdateEvents();
        ListenToAppDeleteEvents();
    }

    private void ListenToPageEvents()
    {
        ListenToPageAddEvents();
        ListenToPageUpdateEvents();
        ListenToPageDeleteEvents();
    }

    private void ListenToPackageEvents()
    {
        ListenToPackageImportEvents();
        ListenToPackageImportCompleteEvents();
    }

    private void ListenToAppAddEvents()
    {
        eventHubBroker.ListenToEvent(eventName: "app_add", handler: (IAppSupportingResourcesCoordinationService service, App app) => service.HandleAppAddAsync(app: app));
        eventHubBroker.ListenToEvent(eventName: "app_add", handler: (IAppRenderableCoordinationService service, App app) => service.HandleAppAddAsync(app: app));
        eventHubBroker.ListenToEvent(eventName: "app_add", handler: (IAppPageComponentCoordinationService service, App app) => service.HandleAppAddAsync(app: app));
    }

    private void ListenToAppUpdateEvents()
    {
        eventHubBroker.ListenToEvent(eventName: "app_update", handler: (IAppSupportingResourcesCoordinationService service, App app) => service.HandleAppUpdateAsync(app: app));
        eventHubBroker.ListenToEvent(eventName: "app_update", handler: (IAppRenderableCoordinationService service, App app) => service.HandleAppUpdateAsync(app: app));
        eventHubBroker.ListenToEvent(eventName: "app_update", handler: (IAppPageComponentCoordinationService service, App app) => service.HandleAppUpdateAsync(app: app));
        eventHubBroker.ListenToEvent(eventName: "app_update", handler: (IPageRenderCacheEventHandlers service, App app) => service.RebuildAppAsync(app: app));
    }

    private void ListenToAppDeleteEvents()
    {
        eventHubBroker.ListenToEvent(eventName: "app_delete", handler: (IAppSupportingResourcesCoordinationService service, App app) => service.HandleAppDeleteAsync(app: app));
        eventHubBroker.ListenToEvent(eventName: "app_delete", handler: (IAppRenderableCoordinationService service, App app) => service.HandleAppDeleteAsync(app: app));
        eventHubBroker.ListenToEvent(eventName: "app_delete", handler: (IAppPageComponentCoordinationService service, App app) => service.HandleAppDeleteAsync(app: app));
        eventHubBroker.ListenToEvent(eventName: "app_delete", handler: (IPageRenderCacheEventHandlers service, App app) => service.DeleteAppAsync(deletedApp: app));
    }

    private void ListenToPageAddEvents()
    {
        eventHubBroker.ListenToEvent(eventName: "page_add", handler: (IPageCoordinationService service, Page page) => service.HandlePageAddAsync(page: page));

        eventHubBroker.ListenToEvent(eventName: "page_add", handler: (IPageStructureCoordinationService service, Page page) => service.HandlePageAddAsync(page: page));
        eventHubBroker.ListenToEvent(eventName: "page_add", handler: (IPageRenderCacheEventHandlers service, Page page) => service.RebuildPageAsync(page: page));
    }

    private void ListenToPageUpdateEvents()
    {
        eventHubBroker.ListenToEvent(eventName: "page_update", handler: (IPageCoordinationService service, Page page) => service.HandlePageUpdateAsync(page: page));

        eventHubBroker.ListenToEvent(eventName: "page_update", handler: (IPageStructureCoordinationService service, Page page) => service.HandlePageUpdateAsync(page: page));
        eventHubBroker.ListenToEvent(eventName: "page_update", handler: (IPageRenderCacheEventHandlers service, Page page) => service.RebuildPageAsync(page: page));
    }

    private void ListenToPageDeleteEvents()
    {
        eventHubBroker.ListenToEvent(eventName: "page_delete", handler: (IPageCoordinationService service, Page page) => service.HandlePageDeleteAsync(page: page));

        eventHubBroker.ListenToEvent(eventName: "page_delete", handler: (IPageStructureCoordinationService service, Page page) => service.HandlePageDeleteAsync(page: page));
        eventHubBroker.ListenToEvent(eventName: "page_delete", handler: (IPageRenderCacheEventHandlers service, Page page) => service.DeletePageAsync(deletedPage: page));
    }

    private void ListenToPageRenderCacheEvents()
    {
        eventHubBroker.ListenToEvent(
            eventName: "page_render_cache_miss",
            handler: (IPageRenderCacheEventHandlers service, PageRenderCacheMiss cacheMiss) =>
                service.RebuildMissingPageAsync(cacheMiss: cacheMiss));

        ListenToAppOwnedRenderEvents<AppCulture>(eventNames: ["app_culture_add", "app_culture_delete"]);
        ListenToAppOwnedRenderEvents<Layout>(eventNames: ["layout_add", "layout_update", "layout_delete"]);
        ListenToAppOwnedRenderEvents<Template>(eventNames: ["template_add", "template_update", "template_delete"]);
        ListenToAppOwnedRenderEvents<Component>(eventNames: ["component_add", "component_update", "component_delete"]);
        ListenToAppOwnedRenderEvents<Resource>(eventNames: ["resource_add", "resource_update", "resource_delete"]);
        ListenToAppOwnedRenderEvents<Script>(eventNames: ["script_add", "script_update", "script_delete"]);

        foreach (string eventName in new[] { "content_add", "content_update", "content_delete" })
        {
            eventHubBroker.ListenToEvent(eventName: eventName, handler: (IPageRenderCacheEventHandlers service, Content content) => service.RebuildPageAsync(content: content));
        }

        foreach (string eventName in new[] { "page_info_add", "page_info_update", "page_info_delete" })
        {
            eventHubBroker.ListenToEvent(eventName: eventName, handler: (IPageRenderCacheEventHandlers service, PageInfo pageInfo) => service.RebuildPageAsync(pageInfo: pageInfo));
        }

        foreach (string eventName in new[] { "common_object_add", "common_object_update", "common_object_delete" })
        {
            eventHubBroker.ListenToEvent(eventName: eventName, handler: (IPageRenderCacheEventHandlers service, CommonObject commonObject) => service.RebuildCommonCacheConsumersAsync(commonObject: commonObject));
        }
    }

    private void ListenToAppOwnedRenderEvents<T>(string[] eventNames)
    {
        foreach (string eventName in eventNames)
        {
            eventHubBroker.ListenToEvent<T, IPageRenderCacheEventHandlers>(
                eventName: eventName,
                handler: static (service, item) => item switch
                {
                    AppCulture appCulture => service.RebuildAppAsync(appCulture: appCulture),
                    Layout layout => service.RebuildAppAsync(layout: layout),
                    Template template => service.RebuildAppAsync(template: template),
                    Component component => service.RebuildAppAsync(component: component),
                    Resource resource => service.RebuildAppAsync(resource: resource),
                    Script script => service.RebuildAppAsync(script: script),
                    _ => ValueTask.CompletedTask
                });
        }
    }

    private void ListenToPackageImportEvents() =>
        eventHubBroker.ListenToEvent(eventName: "package_import", handler: (IContentManagementMigrationAggregationService service, (int appId, Package package) args) => service.ImportPackageAsync(appId: args.appId, package: args.package));

    private void ListenToPackageImportCompleteEvents() =>
        eventHubBroker.ListenToEvent(eventName: "package_import_complete", handler: (IPageRenderCacheEventHandlers service, (int appId, Package package) args) => service.RebuildAppAsync(appId: args.appId));

}