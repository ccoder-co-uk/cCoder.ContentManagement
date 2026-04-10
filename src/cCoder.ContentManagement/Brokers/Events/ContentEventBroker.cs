using cCoder.Data.Models.CMS;
using EventLibrary;
using EventLibrary.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public class ContentEventBroker(IEventHub eventHub) : IContentEventBroker
{
    public ValueTask RaiseContentAddEventAsync(EventMessage<Content> message)
    {
        return eventHub.RaiseEventAsync("content_add", message);
    }

    public ValueTask RaiseContentUpdateEventAsync(EventMessage<Content> message)
    {
        return eventHub.RaiseEventAsync("content_update", message);
    }

    public ValueTask RaiseContentDeleteEventAsync(EventMessage<Content> message)
    {
        return eventHub.RaiseEventAsync("content_delete", message);
    }
}
