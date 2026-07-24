// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public class CultureEventBroker(IEventHub eventHub) : ICultureEventBroker
{
    public ValueTask RaiseCultureAddEventAsync(EventMessage<Culture> message) =>
        eventHub.RaiseEventAsync(name: "culture_add", message: message);

    public ValueTask RaiseCultureUpdateEventAsync(EventMessage<Culture> message) =>
        eventHub.RaiseEventAsync(name: "culture_update", message: message);

    public ValueTask RaiseCultureDeleteEventAsync(EventMessage<Culture> message) =>
        eventHub.RaiseEventAsync(name: "culture_delete", message: message);
}