// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

internal sealed class AppEventBroker(IEventHub eventHub) : IAppEventBroker
{
    public ValueTask RaiseAppAddEventAsync(EventMessage<App> message) =>
        eventHub.RaiseEventAsync(name: "app_add", message: message);

    public ValueTask RaiseAppUpdateEventAsync(EventMessage<App> message) =>
        eventHub.RaiseEventAsync(name: "app_update", message: message);

    public ValueTask RaiseAppDeleteEventAsync(EventMessage<App> message) =>
        eventHub.RaiseEventAsync(name: "app_delete", message: message);
}