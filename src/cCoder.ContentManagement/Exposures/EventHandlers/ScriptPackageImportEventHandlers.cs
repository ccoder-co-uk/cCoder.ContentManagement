// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Brokers.Events;

namespace cCoder.ContentManagement.Exposures.EventHandlers;

internal sealed class ScriptPackageImportEventHandlers(IEventRegistrationBroker eventHub)
    : IContentManagementEventHandlers
{
    public void ListenToAllEvents() =>
        eventHub.ListenToEvent(
            name: "script_import",
            handler: (IScriptOrchestrationService service,
                PackageItemImportEvent<Script> import) =>
                service.ImportScriptsAsync(
                    appId: import.AppId!.Value,
                    items: import.Items));

    public void ListenToWebCacheEvents() { }

    public void ListenToHostedEvents() { }

    public void ListenToFinalAppDeleteEvent() { }
}