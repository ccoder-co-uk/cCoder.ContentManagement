using cCoder.Data.Models.CMS;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public interface IPageEventBroker
{
    ValueTask RaisePageAddEventAsync(EventMessage<Page> message);

    ValueTask RaisePageUpdateEventAsync(EventMessage<Page> message);

    ValueTask RaisePageDeleteEventAsync(EventMessage<Page> message);
}
