// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing;
using cCoder.Eventing.Models;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Brokers.Events;

internal sealed class PageRoleEventBroker(IEventInfrastructureDependency eventInfrastructureDependency)
    : AuthenticatedEventBroker(eventInfrastructureDependency), IPageRoleEventBroker
{
    public ValueTask RaisePageRoleAddEventAsync(EventMessage<PageRole> message) =>
        eventInfrastructureDependency.RaiseEventAsync(name: "page_role_add", message: message);

    public ValueTask RaisePageRoleDeleteEventAsync(EventMessage<PageRole> message) =>
        eventInfrastructureDependency.RaiseEventAsync(name: "page_role_delete", message: message);
}