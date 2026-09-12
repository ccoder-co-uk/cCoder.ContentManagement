// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using cCoder.Eventing;

namespace cCoder.ContentManagement.Exposures.EventHandlers;

internal sealed class ResourcePackageImportEventHandlers(IEventHub eventHub)
    : IContentManagementEventHandlers
{
    public void ListenToAllEvents() =>
        eventHub.ListenToEvent(
            name: "resource_import",
            handler: (IResourceOrchestrationService service,
                PackageItemImportEvent<Resource> import) =>
                service.ImportResourcesAsync(
                    appId: import.AppId!.Value,
                    items: import.Items));

    public void ListenToWebCacheEvents() { }

    public void ListenToHostedEvents() { }

    public void ListenToFinalAppDeleteEvent() { }
}