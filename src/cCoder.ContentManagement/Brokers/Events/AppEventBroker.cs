using cCoder.Data.Models.CMS;
using EventLibrary;
using EventLibrary.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public class AppEventBroker(IEventHub eventHub) : IAppEventBroker
{
    public ValueTask RaiseAppAddEventAsync(EventMessage<App> message)
    {
        return eventHub.RaiseEventAsync("app_add", message);
    }

    public ValueTask RaiseAppUpdateEventAsync(EventMessage<App> message)
    {
        return eventHub.RaiseEventAsync("app_update", message);
    }

    public ValueTask RaiseAppDeleteEventAsync(EventMessage<App> message)
    {
        return eventHub.RaiseEventAsync("app_delete", message);
    }
}
