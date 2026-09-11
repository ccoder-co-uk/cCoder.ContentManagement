// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Coordinations;
using cCoder.Data.Models.CMS;
using cCoder.Eventing;

namespace cCoder.ContentManagement.Exposures.EventHandlers;

internal sealed class AppSupportingResourcesEventHandlers(IEventHub eventHub)
    : IContentManagementEventHandlers
{
    public void ListenToAllEvents()
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

    public void ListenToWebCacheEvents() { }

    public void ListenToHostedEvents() { }

    public void ListenToFinalAppDeleteEvent() { }
}