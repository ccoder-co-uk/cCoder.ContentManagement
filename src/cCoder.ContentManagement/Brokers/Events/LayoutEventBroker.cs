using cCoder.Data.Models.CMS;
using EventLibrary;
using EventLibrary.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public class LayoutEventBroker(IEventHub eventHub) : ILayoutEventBroker
{
    public ValueTask RaiseLayoutAddEventAsync(EventMessage<Layout> message)
    {
        return eventHub.RaiseEventAsync("layout_add", message);
    }

    public ValueTask RaiseLayoutUpdateEventAsync(EventMessage<Layout> message)
    {
        return eventHub.RaiseEventAsync("layout_update", message);
    }

    public ValueTask RaiseLayoutDeleteEventAsync(EventMessage<Layout> message)
    {
        return eventHub.RaiseEventAsync("layout_delete", message);
    }
}
