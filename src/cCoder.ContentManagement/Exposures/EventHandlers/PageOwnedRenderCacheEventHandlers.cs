// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Eventing;

namespace cCoder.ContentManagement.Exposures.EventHandlers;

internal sealed class PageOwnedRenderCacheEventHandlers(IEventHub eventHub)
    : IContentManagementEventHandlers
{
    public void ListenToAllEvents() { }

    public void ListenToWebCacheEvents()
    {
        eventHub.ListenToEvent(
            name: "page_add",
            handler: (IPageRenderCacheEventHandlers service, Page page) =>
                service.InvalidatePageAsync(page: page));

        eventHub.ListenToEvent(
            name: "page_update",
            handler: (IPageRenderCacheEventHandlers service, Page page) =>
                service.InvalidatePageAsync(page: page));

        eventHub.ListenToEvent(
            name: "page_delete",
            handler: (IPageRenderCacheEventHandlers service, Page page) =>
                service.DeletePageAsync(deletedPage: page));

        eventHub.ListenToEvent(
            name: "content_add",
            handler: (IPageRenderCacheEventHandlers service, Content content) =>
                service.InvalidatePageAsync(content: content));

        eventHub.ListenToEvent(
            name: "content_update",
            handler: (IPageRenderCacheEventHandlers service, Content content) =>
                service.InvalidatePageAsync(content: content));

        eventHub.ListenToEvent(
            name: "content_delete",
            handler: (IPageRenderCacheEventHandlers service, Content content) =>
                service.InvalidatePageAsync(content: content));

        eventHub.ListenToEvent(
            name: "page_info_add",
            handler: (IPageRenderCacheEventHandlers service, PageInfo pageInfo) =>
                service.InvalidatePageAsync(pageInfo: pageInfo));

        eventHub.ListenToEvent(
            name: "page_info_update",
            handler: (IPageRenderCacheEventHandlers service, PageInfo pageInfo) =>
                service.InvalidatePageAsync(pageInfo: pageInfo));

        eventHub.ListenToEvent(
            name: "page_info_delete",
            handler: (IPageRenderCacheEventHandlers service, PageInfo pageInfo) =>
                service.InvalidatePageAsync(pageInfo: pageInfo));
    }

    public void ListenToHostedEvents() { }

    public void ListenToFinalAppDeleteEvent() { }
}