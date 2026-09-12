// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Coordinations;
using cCoder.Data.Models.CMS;
using cCoder.Eventing;

namespace cCoder.ContentManagement.Exposures.EventHandlers;

internal sealed class PagePackageImportEventHandlers(IEventHub eventHub)
    : IContentManagementEventHandlers
{
    public void ListenToAllEvents() =>
        eventHub.ListenToEvent(
            name: "page_import",
            handler: (IPagePackageImportCoordinationService service,
                PackageItemImportEvent<Page> import) =>
                service.ImportPagesAsync(
                    appId: import.AppId!.Value,
                    pages: import.Items));

    public void ListenToWebCacheEvents() { }

    public void ListenToHostedEvents() { }

    public void ListenToFinalAppDeleteEvent() { }
}