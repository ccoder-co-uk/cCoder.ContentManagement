using cCoder.Data.Models.CMS;
using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public class ComponentEventBroker(IEventHub eventHub) : IComponentEventBroker
{
    public ValueTask RaiseComponentAddEventAsync(EventMessage<Component> message)
    {
        return eventHub.RaiseEventAsync("component_add", message);
    }

    public ValueTask RaiseComponentUpdateEventAsync(EventMessage<Component> message)
    {
        return eventHub.RaiseEventAsync("component_update", message);
    }

    public ValueTask RaiseComponentDeleteEventAsync(EventMessage<Component> message)
    {
        return eventHub.RaiseEventAsync("component_delete", message);
    }
}
