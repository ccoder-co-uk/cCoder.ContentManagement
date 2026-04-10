using cCoder.Data.Models.CMS;
using EventLibrary;
using EventLibrary.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public class ResourceEventBroker(IEventHub eventHub) : IResourceEventBroker
{
    public ValueTask RaiseResourceAddEventAsync(EventMessage<Resource> message)
    {
        return eventHub.RaiseEventAsync("resource_add", message);
    }

    public ValueTask RaiseResourceUpdateEventAsync(EventMessage<Resource> message)
    {
        return eventHub.RaiseEventAsync("resource_update", message);
    }

    public ValueTask RaiseResourceDeleteEventAsync(EventMessage<Resource> message)
    {
        return eventHub.RaiseEventAsync("resource_delete", message);
    }
}
