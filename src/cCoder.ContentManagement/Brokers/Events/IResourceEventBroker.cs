using cCoder.Data.Models.CMS;
using EventLibrary.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public interface IResourceEventBroker
{
    ValueTask RaiseResourceAddEventAsync(EventMessage<Resource> message);

    ValueTask RaiseResourceUpdateEventAsync(EventMessage<Resource> message);

    ValueTask RaiseResourceDeleteEventAsync(EventMessage<Resource> message);
}
