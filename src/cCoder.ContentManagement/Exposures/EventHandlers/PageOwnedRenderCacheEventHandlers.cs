// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Brokers.Events;
using cCoder.ContentManagement.Services.Aggregations;

namespace cCoder.ContentManagement.Exposures.EventHandlers;

internal sealed class PageOwnedRenderCacheEventHandlers(IEventRegistrationBroker eventHub)
    : IContentManagementEventHandlers
{
    public void ListenToAllEvents() { }

    public void ListenToWebCacheEvents()
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

    public void ListenToHostedEvents() { }

    public void ListenToFinalAppDeleteEvent() { }
}