using cCoder.Eventing.Models;
using PageInfo = cCoder.Data.Models.CMS.PageInfo;

namespace cCoder.ContentManagement.Brokers.Events;

public interface IPageInfoEventBroker
{
    ValueTask RaisePageInfoAddEventAsync(EventMessage<PageInfo> message);

    ValueTask RaisePageInfoUpdateEventAsync(EventMessage<PageInfo> message);

    ValueTask RaisePageInfoDeleteEventAsync(EventMessage<PageInfo> message);
}
