using cCoder.Eventing.Models;
using CommonObject = cCoder.Data.Models.CommonObject;

namespace cCoder.ContentManagement.Brokers.Events;

public interface ICommonObjectEventBroker
{
    ValueTask RaiseCommonObjectAddEventAsync(EventMessage<CommonObject> message);

    ValueTask RaiseCommonObjectUpdateEventAsync(EventMessage<CommonObject> message);

    ValueTask RaiseCommonObjectDeleteEventAsync(EventMessage<CommonObject> message);
}
