using cCoder.Data.Models.CMS;
using EventLibrary.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public interface IAppEventBroker
{
    ValueTask RaiseAppAddEventAsync(EventMessage<App> message);

    ValueTask RaiseAppDeleteEventAsync(EventMessage<App> message);

    ValueTask RaiseAppUpdateEventAsync(EventMessage<App> message);
}
