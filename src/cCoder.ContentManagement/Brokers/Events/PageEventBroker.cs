// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public class PageEventBroker(IEventHub eventHub) : IPageEventBroker
{
    public ValueTask RaisePageAddEventAsync(EventMessage<Page> message) =>
        eventHub.RaiseEventAsync(name: "page_add", message: message);

    public ValueTask RaisePageUpdateEventAsync(EventMessage<Page> message) =>
        eventHub.RaiseEventAsync(name: "page_update", message: message);

    public ValueTask RaisePageDeleteEventAsync(EventMessage<Page> message) =>
        eventHub.RaiseEventAsync(name: "page_delete", message: message);
}