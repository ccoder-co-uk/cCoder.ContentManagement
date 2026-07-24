// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Coordinations;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.CMS;

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

    private void ListenToPackageEvents() =>
        ListenToPackageImportEvents();

    private void ListenToAppAddEvents()
    {
        eventHubBroker.ListenToEvent(eventName: "app_add", handler: (IAppSupportingResourcesCoordinationService service, App app) => service.HandleAppAddAsync(app: app));
        eventHubBroker.ListenToEvent(eventName: "app_add", handler: (IAppRenderableCoordinationService service, App app) => service.HandleAppAddAsync(app: app));
    }

    private void ListenToAppUpdateEvents()
    {
        eventHubBroker.ListenToEvent(eventName: "app_update", handler: (IAppSupportingResourcesCoordinationService service, App app) => service.HandleAppUpdateAsync(app: app));
        eventHubBroker.ListenToEvent(eventName: "app_update", handler: (IAppRenderableCoordinationService service, App app) => service.HandleAppUpdateAsync(app: app));
    }

    private void ListenToAppDeleteEvents()
    {
        eventHubBroker.ListenToEvent(eventName: "app_delete", handler: (IAppSupportingResourcesCoordinationService service, App app) => service.HandleAppDeleteAsync(app: app));
        eventHubBroker.ListenToEvent(eventName: "app_delete", handler: (IAppRenderableCoordinationService service, App app) => service.HandleAppDeleteAsync(app: app));
    }

    private void ListenToPageAddEvents() =>
        eventHubBroker.ListenToEvent(eventName: "page_add", handler: (IPageCoordinationService service, Page page) => service.HandlePageAddAsync(page: page));

    private void ListenToPageUpdateEvents() =>
        eventHubBroker.ListenToEvent(eventName: "page_update", handler: (IPageCoordinationService service, Page page) => service.HandlePageUpdateAsync(page: page));

    private void ListenToPageDeleteEvents() =>
        eventHubBroker.ListenToEvent(eventName: "page_delete", handler: (IPageCoordinationService service, Page page) => service.HandlePageDeleteAsync(page: page));

    private void ListenToPackageImportEvents() =>
        eventHubBroker.ListenToEvent(eventName: "package_import", handler: (IContentManagementMigrationAggregationService service, (int appId, Package package) args) => service.ImportPackageAsync(appId: args.appId, package: args.package));

}