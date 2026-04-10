using EventLibrary;
using EventLibrary.Models;
using PageInfo = cCoder.Data.Models.CMS.PageInfo;

namespace cCoder.ContentManagement.Brokers.Events;

public class PageInfoEventBroker(IEventHub eventHub) : IPageInfoEventBroker
{
    public ValueTask RaisePageInfoAddEventAsync(EventMessage<PageInfo> message)
    {
        return eventHub.RaiseEventAsync("page_info_add", message);
    }

    public ValueTask RaisePageInfoUpdateEventAsync(EventMessage<PageInfo> message)
    {
        return eventHub.RaiseEventAsync("page_info_update", message);
    }

    public ValueTask RaisePageInfoDeleteEventAsync(EventMessage<PageInfo> message)
    {
        return eventHub.RaiseEventAsync("page_info_delete", message);
    }
}
