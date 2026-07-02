using cCoder.Eventing;
using cCoder.Eventing.Models;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public class CommonObjectEventBroker(IEventHub eventHub) : ICommonObjectEventBroker
{
    public ValueTask RaiseCommonObjectAddEventAsync(EventMessage<CommonObject> message) =>
        eventHub.RaiseEventAsync("common_object_add", message);

    public ValueTask RaiseCommonObjectUpdateEventAsync(EventMessage<CommonObject> message) =>
        eventHub.RaiseEventAsync("common_object_update", message);

    public ValueTask RaiseCommonObjectDeleteEventAsync(EventMessage<CommonObject> message) =>
        eventHub.RaiseEventAsync("common_object_delete", message);
}
