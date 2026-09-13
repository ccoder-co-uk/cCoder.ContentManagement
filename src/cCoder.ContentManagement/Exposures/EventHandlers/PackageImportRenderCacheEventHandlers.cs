// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Brokers.Events;
using cCoder.ContentManagement.Services.Aggregations;

namespace cCoder.ContentManagement.Exposures.EventHandlers;

internal sealed class PackageImportRenderCacheEventHandlers(IEventRegistrationBroker eventHub)
    : IContentManagementEventHandlers
{
    public void ListenToAllEvents() { }

    public void ListenToWebCacheEvents() =>
        eventHub.ListenToEvent(
            name: "package_import_complete",
            handler: (IPageRenderCacheAggregationService service, PackageImportEvent args) =>
                service.InvalidatePackageAsync(appId: args.AppId));

    public void ListenToHostedEvents() { }

    public void ListenToFinalAppDeleteEvent() { }
}