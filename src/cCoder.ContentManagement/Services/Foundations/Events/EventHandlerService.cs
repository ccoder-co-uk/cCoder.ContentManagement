using cCoder.ContentManagement.Brokers.Events;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Coordinations;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class EventHandlerService(IEventHubBroker eventHubBroker) : IEventHandlerService
{
    public void ListenToAllEvents()
    {
        ValidateEventHubBroker(eventHubBroker, "eventHubBroker");
        ListenToAppEvents();
        ListenToPageEvents();
        ListenToPackageEvents();
    }

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
    }

    private void ListenToAppAddEvents()
    {
        eventHubBroker.ListenToEvent("app_add", (IAppSupportingResourcesCoordinationService service, App app) => service.HandleAppAddAsync(app));
        eventHubBroker.ListenToEvent("app_add", (IAppRenderableCoordinationService service, App app) => service.HandleAppAddAsync(app));
    }

    private void ListenToAppUpdateEvents()
    {
        eventHubBroker.ListenToEvent("app_update", (IAppSupportingResourcesCoordinationService service, App app) => service.HandleAppUpdateAsync(app));
        eventHubBroker.ListenToEvent("app_update", (IAppRenderableCoordinationService service, App app) => service.HandleAppUpdateAsync(app));
    }

    private void ListenToAppDeleteEvents()
    {
        eventHubBroker.ListenToEvent("app_delete", (IAppSupportingResourcesCoordinationService service, App app) => service.HandleAppDeleteAsync(app));
        eventHubBroker.ListenToEvent("app_delete", (IAppRenderableCoordinationService service, App app) => service.HandleAppDeleteAsync(app));
    }

    private void ListenToPageAddEvents()
    {
        eventHubBroker.ListenToEvent("page_add", (IPageCoordinationService service, Page page) => service.HandlePageAddAsync(page));
    }

    private void ListenToPageUpdateEvents()
    {
        eventHubBroker.ListenToEvent("page_update", (IPageCoordinationService service, Page page) => service.HandlePageUpdateAsync(page));
    }

    private void ListenToPageDeleteEvents()
    {
        eventHubBroker.ListenToEvent("page_delete", (IPageCoordinationService service, Page page) => service.HandlePageDeleteAsync(page));
    }

    private void ListenToPackageImportEvents()
    {
        eventHubBroker.ListenToEvent("package_import", (IContentManagementMigrationAggregationService service, (int appId, Package package) args) => service.ImportPackageAsync(args.appId, args.package));
    }
}