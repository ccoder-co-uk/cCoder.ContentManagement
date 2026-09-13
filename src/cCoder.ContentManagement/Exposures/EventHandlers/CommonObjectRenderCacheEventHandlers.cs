// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.ContentManagement.Brokers.Events;
using cCoder.ContentManagement.Services.Aggregations;

namespace cCoder.ContentManagement.Exposures.EventHandlers;

internal sealed class CommonObjectRenderCacheEventHandlers(IEventRegistrationBroker eventHub)
    : IContentManagementEventHandlers
{
    public void ListenToAllEvents() { }

    public void ListenToWebCacheEvents()
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

    public void ListenToHostedEvents() { }

    public void ListenToFinalAppDeleteEvent() { }
}