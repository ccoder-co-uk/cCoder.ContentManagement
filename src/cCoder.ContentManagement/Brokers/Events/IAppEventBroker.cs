using cCoder.Data.Models.CMS;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public interface IAppEventBroker
{
    ValueTask RaiseAppAddEventAsync(EventMessage<App> message);

    ValueTask RaiseAppDeleteEventAsync(EventMessage<App> message);

    ValueTask RaiseAppUpdateEventAsync(EventMessage<App> message);
}
