using cCoder.Eventing;
using cCoder.Eventing.Models;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Brokers.Events;

public class PageRoleEventBroker(IEventHub eventHub) : IPageRoleEventBroker
{
    public ValueTask RaisePageRoleAddEventAsync(EventMessage<PageRole> message) =>
        eventHub.RaiseEventAsync("page_role_add", message);

    public ValueTask RaisePageRoleDeleteEventAsync(EventMessage<PageRole> message) =>
        eventHub.RaiseEventAsync("page_role_delete", message);
}
