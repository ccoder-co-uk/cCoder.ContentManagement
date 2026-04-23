using cCoder.Data.Models.CMS;
using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public class AppCultureEventBroker(IEventHub eventHub) : IAppCultureEventBroker
{
    public ValueTask RaiseAppCultureAddEventAsync(EventMessage<AppCulture> message)
    {
        return eventHub.RaiseEventAsync("app_culture_add", message);
    }

    public ValueTask RaiseAppCultureDeleteEventAsync(EventMessage<AppCulture> message)
    {
        return eventHub.RaiseEventAsync("app_culture_delete", message);
    }
}
