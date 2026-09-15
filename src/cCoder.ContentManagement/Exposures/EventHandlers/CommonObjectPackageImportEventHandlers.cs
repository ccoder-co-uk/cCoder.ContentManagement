// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Data.Models;
using cCoder.ContentManagement.Brokers.Events;
using cCoder.ContentManagement.Services.Coordinations;

namespace cCoder.ContentManagement.Exposures.EventHandlers;

internal sealed class CommonObjectPackageImportEventHandlers(IEventRegistrationBroker eventHub)
    : IContentManagementEventHandlers
{
    public void ListenToAllEvents() =>
        eventHub.ListenToEvent(
            name: "common_objects_import",
            handler: (ICommonObjectCoordinationService service,
                PackageItemImportEvent<CommonObject> import) =>
                ImportCommonObjectsAsync(
                    service: service,
                    items: import.Items));

    public void ListenToWebCacheEvents() { }

    public void ListenToHostedEvents() { }

    public void ListenToFinalAppDeleteEvent() { }

    private static async ValueTask ImportCommonObjectsAsync(
        ICommonObjectCoordinationService service,
        CommonObject[] items) =>
        _ = await service.AddAllCommonObjectsAsync(
            newCommonObjects: items);
}