// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

internal sealed class PageEventBroker(IAuthenticatedEventHub eventHub)
    : AuthenticatedEventBroker(eventHub), IPageEventBroker
{
    public ValueTask RaisePageAddEventAsync(EventMessage<Page> message) =>
        RaiseEventAsync(name: "page_add", message: message);

    public ValueTask RaisePageUpdateEventAsync(EventMessage<Page> message) =>
        RaiseEventAsync(name: "page_update", message: message);

    public ValueTask RaisePageDeleteEventAsync(EventMessage<Page> message) =>
        RaiseEventAsync(name: "page_delete", message: message);
}