using cCoder.Data.Models.CMS;
using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public class PageEventBroker(IEventHub eventHub) : IPageEventBroker
{
    public ValueTask RaisePageAddEventAsync(EventMessage<Page> message)
    {
        return eventHub.RaiseEventAsync("page_add", message);
    }

    public ValueTask RaisePageUpdateEventAsync(EventMessage<Page> message)
    {
        return eventHub.RaiseEventAsync("page_update", message);
    }

    public ValueTask RaisePageDeleteEventAsync(EventMessage<Page> message)
    {
        return eventHub.RaiseEventAsync("page_delete", message);
    }
}
