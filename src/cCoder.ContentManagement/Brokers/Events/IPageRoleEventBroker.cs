using cCoder.Eventing.Models;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Brokers.Events;

public interface IPageRoleEventBroker
{
    ValueTask RaisePageRoleAddEventAsync(EventMessage<PageRole> message);

    ValueTask RaisePageRoleDeleteEventAsync(EventMessage<PageRole> message);
}
