// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Eventing;

namespace cCoder.ContentManagement.Exposures.EventHandlers;

internal sealed class PackageImportRenderCacheEventHandlers(IEventHub eventHub)
    : IContentManagementEventHandlers
{
    public void ListenToAllEvents() { }

    public void ListenToWebCacheEvents() =>
        eventHub.ListenToEvent(
            name: "package_import_complete",
            handler: (IPageRenderCacheEventHandlers service, PackageImportEvent args) =>
                service.InvalidatePackageAsync(appId: args.AppId));

    public void ListenToHostedEvents() { }

    public void ListenToFinalAppDeleteEvent() { }
}