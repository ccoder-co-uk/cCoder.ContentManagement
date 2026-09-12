// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using cCoder.Eventing;

namespace cCoder.ContentManagement.Exposures.EventHandlers;

internal sealed class TemplatePackageImportEventHandlers(IEventHub eventHub)
    : IContentManagementEventHandlers
{
    public void ListenToAllEvents() =>
        eventHub.ListenToEvent(
            name: "template_import",
            handler: (ITemplateOrchestrationService service,
                PackageItemImportEvent<Template> import) =>
                service.ImportTemplatesAsync(
                    appId: import.AppId!.Value,
                    items: import.Items));

    public void ListenToWebCacheEvents() { }

    public void ListenToHostedEvents() { }

    public void ListenToFinalAppDeleteEvent() { }
}