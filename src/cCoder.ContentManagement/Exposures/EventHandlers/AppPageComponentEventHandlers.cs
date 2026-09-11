// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Coordinations;
using cCoder.Data.Models.CMS;
using cCoder.Eventing;

namespace cCoder.ContentManagement.Exposures.EventHandlers;

internal sealed class AppPageComponentEventHandlers(IEventHub eventHub)
    : IContentManagementEventHandlers
{
    public void ListenToAllEvents()
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

    public void ListenToWebCacheEvents() { }

    public void ListenToHostedEvents() { }

    public void ListenToFinalAppDeleteEvent() { }
}