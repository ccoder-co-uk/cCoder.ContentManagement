// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing;
using cCoder.Eventing.Models;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Brokers.Events;

internal sealed class CommonObjectEventBroker(IAuthenticatedEventHub eventHub)
    : AuthenticatedEventBroker(eventHub), ICommonObjectEventBroker
{
    public ValueTask RaiseCommonObjectAddEventAsync(EventMessage<CommonObject> message) =>
        RaiseEventAsync(name: "common_object_add", message: message);

    public ValueTask RaiseCommonObjectUpdateEventAsync(EventMessage<CommonObject> message) =>
        RaiseEventAsync(name: "common_object_update", message: message);

    public ValueTask RaiseCommonObjectDeleteEventAsync(EventMessage<CommonObject> message) =>
        RaiseEventAsync(name: "common_object_delete", message: message);

    public ValueTask RaiseCommonObjectsImportedEventAsync(
        EventMessage<CommonObject[]> message) =>
        RaiseEventAsync(
            name: "common_objects_imported",
            message: message);
}