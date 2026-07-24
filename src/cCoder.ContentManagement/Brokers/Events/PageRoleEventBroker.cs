// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing;
using cCoder.Eventing.Models;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Brokers.Events;

public class PageRoleEventBroker(IEventHub eventHub) : IPageRoleEventBroker
{
    public ValueTask RaisePageRoleAddEventAsync(EventMessage<PageRole> message) =>
        eventHub.RaiseEventAsync(name: "page_role_add", message: message);

    public ValueTask RaisePageRoleDeleteEventAsync(EventMessage<PageRole> message) =>
        eventHub.RaiseEventAsync(name: "page_role_delete", message: message);
}