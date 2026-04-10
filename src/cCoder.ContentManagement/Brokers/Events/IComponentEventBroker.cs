using cCoder.Data.Models.CMS;
using EventLibrary.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public interface IComponentEventBroker
{
    ValueTask RaiseComponentAddEventAsync(EventMessage<Component> message);

    ValueTask RaiseComponentUpdateEventAsync(EventMessage<Component> message);

    ValueTask RaiseComponentDeleteEventAsync(EventMessage<Component> message);
}
