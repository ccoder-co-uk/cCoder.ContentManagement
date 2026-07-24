// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

internal sealed class ResourceEventBroker(IEventHub eventHub) : IResourceEventBroker
{
    public ValueTask RaiseResourceAddEventAsync(EventMessage<Resource> message) =>
        eventHub.RaiseEventAsync(name: "resource_add", message: message);

    public ValueTask RaiseResourceUpdateEventAsync(EventMessage<Resource> message) =>
        eventHub.RaiseEventAsync(name: "resource_update", message: message);

    public ValueTask RaiseResourceDeleteEventAsync(EventMessage<Resource> message) =>
        eventHub.RaiseEventAsync(name: "resource_delete", message: message);
}