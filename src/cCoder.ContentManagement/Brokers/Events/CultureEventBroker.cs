using cCoder.Data.Models.CMS;
using EventLibrary;
using EventLibrary.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public class CultureEventBroker(IEventHub eventHub) : ICultureEventBroker
{
    public ValueTask RaiseCultureAddEventAsync(EventMessage<Culture> message)
    {
        return eventHub.RaiseEventAsync("culture_add", message);
    }

    public ValueTask RaiseCultureUpdateEventAsync(EventMessage<Culture> message)
    {
        return eventHub.RaiseEventAsync("culture_update", message);
    }

    public ValueTask RaiseCultureDeleteEventAsync(EventMessage<Culture> message)
    {
        return eventHub.RaiseEventAsync("culture_delete", message);
    }
}
