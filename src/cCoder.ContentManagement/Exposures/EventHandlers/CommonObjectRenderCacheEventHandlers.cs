// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Eventing;

namespace cCoder.ContentManagement.Exposures.EventHandlers;

internal sealed class CommonObjectRenderCacheEventHandlers(IEventHub eventHub)
    : IContentManagementEventHandlers
{
    public void ListenToAllEvents() { }

    public void ListenToWebCacheEvents()
    {
        eventHub.ListenToEvent(
            name: "common_object_add",
            handler: (IPageRenderCacheEventHandlers service, CommonObject commonObject) =>
                service.InvalidateCommonCacheConsumersAsync(commonObject: commonObject));

        eventHub.ListenToEvent(
            name: "common_object_update",
            handler: (IPageRenderCacheEventHandlers service, CommonObject commonObject) =>
                service.InvalidateCommonCacheConsumersAsync(commonObject: commonObject));

        eventHub.ListenToEvent(
            name: "common_object_delete",
            handler: (IPageRenderCacheEventHandlers service, CommonObject commonObject) =>
                service.InvalidateCommonCacheConsumersAsync(commonObject: commonObject));

        eventHub.ListenToEvent(
            name: "common_objects_imported",
            handler: (IPageRenderCacheEventHandlers service, CommonObject[] commonObjects) =>
                service.InvalidateCommonObjectsAsync(commonObjects: commonObjects));
    }

    public void ListenToHostedEvents() { }

    public void ListenToFinalAppDeleteEvent() { }
}