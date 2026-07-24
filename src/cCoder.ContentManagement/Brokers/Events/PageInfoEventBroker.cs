// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing;
using cCoder.Eventing.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Events;

public class PageInfoEventBroker(IEventHub eventHub) : IPageInfoEventBroker
{
    public ValueTask RaisePageInfoAddEventAsync(EventMessage<PageInfo> message) =>
        eventHub.RaiseEventAsync(name: "page_info_add", message: message);

    public ValueTask RaisePageInfoUpdateEventAsync(EventMessage<PageInfo> message) =>
        eventHub.RaiseEventAsync(name: "page_info_update", message: message);

    public ValueTask RaisePageInfoDeleteEventAsync(EventMessage<PageInfo> message) =>
        eventHub.RaiseEventAsync(name: "page_info_delete", message: message);
}