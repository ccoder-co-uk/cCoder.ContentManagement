using cCoder.Data.Models.CMS;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public interface ILayoutEventBroker
{
    ValueTask RaiseLayoutAddEventAsync(EventMessage<Layout> message);

    ValueTask RaiseLayoutUpdateEventAsync(EventMessage<Layout> message);

    ValueTask RaiseLayoutDeleteEventAsync(EventMessage<Layout> message);
}
