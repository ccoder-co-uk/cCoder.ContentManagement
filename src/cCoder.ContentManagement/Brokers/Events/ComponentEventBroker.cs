// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

internal sealed class ComponentEventBroker(IEventHub eventHub)
    : IComponentEventBroker
{
    public ValueTask RaiseComponentAddEventAsync(EventMessage<Component> message) =>
        eventHub.RaiseEventAsync(name: "component_add", message: message);

    public ValueTask RaiseComponentUpdateEventAsync(EventMessage<Component> message) =>
        eventHub.RaiseEventAsync(name: "component_update", message: message);

    public ValueTask RaiseComponentDeleteEventAsync(EventMessage<Component> message) =>
        eventHub.RaiseEventAsync(name: "component_delete", message: message);
}