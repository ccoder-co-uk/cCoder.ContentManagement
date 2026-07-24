// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing;
using cCoder.Eventing.Models;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public class CommonObjectEventBroker(IEventHub eventHub) : ICommonObjectEventBroker
{
    public ValueTask RaiseCommonObjectAddEventAsync(EventMessage<CommonObject> message) =>
        eventHub.RaiseEventAsync(name: "common_object_add", message: message);

    public ValueTask RaiseCommonObjectUpdateEventAsync(EventMessage<CommonObject> message) =>
        eventHub.RaiseEventAsync(name: "common_object_update", message: message);

    public ValueTask RaiseCommonObjectDeleteEventAsync(EventMessage<CommonObject> message) =>
        eventHub.RaiseEventAsync(name: "common_object_delete", message: message);
}