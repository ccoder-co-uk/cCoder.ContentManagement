// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

internal sealed class LayoutEventBroker(IAuthenticatedEventHub eventHub)
    : AuthenticatedEventBroker(eventHub), ILayoutEventBroker
{
    public ValueTask RaiseLayoutAddEventAsync(EventMessage<Layout> message) =>
        RaiseEventAsync(name: "layout_add", message: message);

    public ValueTask RaiseLayoutUpdateEventAsync(EventMessage<Layout> message) =>
        RaiseEventAsync(name: "layout_update", message: message);

    public ValueTask RaiseLayoutDeleteEventAsync(EventMessage<Layout> message) =>
        RaiseEventAsync(name: "layout_delete", message: message);
}