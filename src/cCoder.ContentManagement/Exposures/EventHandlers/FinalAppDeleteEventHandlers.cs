// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Brokers.Events;

namespace cCoder.ContentManagement.Exposures.EventHandlers;

internal sealed class FinalAppDeleteEventHandlers(IEventRegistrationBroker eventHub)
    : IContentManagementEventHandlers
{
    public void ListenToAllEvents() { }

    public void ListenToWebCacheEvents() { }

    public void ListenToHostedEvents() { }

    public void ListenToFinalAppDeleteEvent() =>
        eventHub.ListenToEvent(
            name: "app_delete",
            handler: (IAppOrchestrationService service, App app) =>
                service.HandleAppDeleteAsync(app: app));
}