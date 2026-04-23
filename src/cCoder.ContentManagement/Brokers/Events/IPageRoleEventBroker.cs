using cCoder.Eventing.Models;
using PageRole = cCoder.Data.Models.Security.PageRole;

namespace cCoder.ContentManagement.Brokers.Events;

public interface IPageRoleEventBroker
{
    ValueTask RaisePageRoleAddEventAsync(EventMessage<PageRole> message);

    ValueTask RaisePageRoleDeleteEventAsync(EventMessage<PageRole> message);
}
