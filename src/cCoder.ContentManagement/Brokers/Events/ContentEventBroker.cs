// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

internal sealed class ContentEventBroker(IEventHub eventHub) : IContentEventBroker
{
    public ValueTask RaiseContentAddEventAsync(EventMessage<Content> message) =>
        eventHub.RaiseEventAsync(name: "content_add", message: message);

    public ValueTask RaiseContentUpdateEventAsync(EventMessage<Content> message) =>
        eventHub.RaiseEventAsync(name: "content_update", message: message);

    public ValueTask RaiseContentDeleteEventAsync(EventMessage<Content> message) =>
        eventHub.RaiseEventAsync(name: "content_delete", message: message);
}