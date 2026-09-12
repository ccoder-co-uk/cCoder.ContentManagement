// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Coordinations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Brokers.Events;

namespace cCoder.ContentManagement.Exposures.EventHandlers;

internal sealed class PageStructureEventHandlers(IEventRegistrationBroker eventHub)
    : IContentManagementEventHandlers
{
    public void ListenToAllEvents()
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

    public void ListenToWebCacheEvents() { }

    public void ListenToHostedEvents() { }

    public void ListenToFinalAppDeleteEvent() { }
}