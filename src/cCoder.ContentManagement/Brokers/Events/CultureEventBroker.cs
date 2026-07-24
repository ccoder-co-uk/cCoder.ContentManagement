// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

internal sealed class CultureEventBroker(IEventInfrastructureDependency eventInfrastructureDependency)
    : AuthenticatedEventBroker(eventInfrastructureDependency), ICultureEventBroker
{
    public ValueTask RaiseCultureAddEventAsync(EventMessage<Culture> message) =>
        eventInfrastructureDependency.RaiseEventAsync(name: "culture_add", message: message);

    public ValueTask RaiseCultureUpdateEventAsync(EventMessage<Culture> message) =>
        eventInfrastructureDependency.RaiseEventAsync(name: "culture_update", message: message);

    public ValueTask RaiseCultureDeleteEventAsync(EventMessage<Culture> message) =>
        eventInfrastructureDependency.RaiseEventAsync(name: "culture_delete", message: message);
}