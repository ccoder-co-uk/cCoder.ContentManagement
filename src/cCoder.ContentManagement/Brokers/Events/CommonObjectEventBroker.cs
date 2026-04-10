using EventLibrary;
using EventLibrary.Models;
using CommonObject = cCoder.Data.Models.CommonObject;

namespace cCoder.ContentManagement.Brokers.Events;

public class CommonObjectEventBroker(IEventHub eventHub) : ICommonObjectEventBroker
{
    public ValueTask RaiseCommonObjectAddEventAsync(EventMessage<CommonObject> message)
    {
        return eventHub.RaiseEventAsync("common_object_add", message);
    }

    public ValueTask RaiseCommonObjectUpdateEventAsync(EventMessage<CommonObject> message)
    {
        return eventHub.RaiseEventAsync("common_object_update", message);
    }

    public ValueTask RaiseCommonObjectDeleteEventAsync(EventMessage<CommonObject> message)
    {
        return eventHub.RaiseEventAsync("common_object_delete", message);
    }
}
