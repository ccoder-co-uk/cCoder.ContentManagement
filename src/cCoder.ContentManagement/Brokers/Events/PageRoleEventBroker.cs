using cCoder.Eventing;
using cCoder.Eventing.Models;
using PageRole = cCoder.Data.Models.Security.PageRole;

namespace cCoder.ContentManagement.Brokers.Events;

public class PageRoleEventBroker(IEventHub eventHub) : IPageRoleEventBroker
{
    public ValueTask RaisePageRoleAddEventAsync(EventMessage<PageRole> message)
    {
        return eventHub.RaiseEventAsync("page_role_add", message);
    }

    public ValueTask RaisePageRoleDeleteEventAsync(EventMessage<PageRole> message)
    {
        return eventHub.RaiseEventAsync("page_role_delete", message);
    }
}
