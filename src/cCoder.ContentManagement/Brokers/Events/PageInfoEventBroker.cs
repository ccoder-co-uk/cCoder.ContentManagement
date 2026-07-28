// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing;
using cCoder.Eventing.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Events;

internal sealed class PageInfoEventBroker(IAuthenticatedEventHub eventHub)
    : AuthenticatedEventBroker(eventHub), IPageInfoEventBroker
{
    public ValueTask RaisePageInfoAddEventAsync(EventMessage<PageInfo> message) =>
        RaiseEventAsync(name: "page_info_add", message: message);

    public ValueTask RaisePageInfoUpdateEventAsync(EventMessage<PageInfo> message) =>
        RaiseEventAsync(name: "page_info_update", message: message);

    public ValueTask RaisePageInfoDeleteEventAsync(EventMessage<PageInfo> message) =>
        RaiseEventAsync(name: "page_info_delete", message: message);
}