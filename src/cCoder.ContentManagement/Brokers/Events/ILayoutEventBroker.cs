using cCoder.Data.Models.CMS;
using EventLibrary.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public interface ILayoutEventBroker
{
    ValueTask RaiseLayoutAddEventAsync(EventMessage<Layout> message);

    ValueTask RaiseLayoutUpdateEventAsync(EventMessage<Layout> message);

    ValueTask RaiseLayoutDeleteEventAsync(EventMessage<Layout> message);
}
