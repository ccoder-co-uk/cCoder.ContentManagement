using cCoder.Eventing.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Events;

public interface IPageInfoEventBroker
{
    ValueTask RaisePageInfoAddEventAsync(EventMessage<PageInfo> message);

    ValueTask RaisePageInfoUpdateEventAsync(EventMessage<PageInfo> message);

    ValueTask RaisePageInfoDeleteEventAsync(EventMessage<PageInfo> message);
}
