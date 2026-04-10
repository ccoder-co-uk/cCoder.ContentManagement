using cCoder.Data.Models.CMS;
using EventLibrary.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public interface ICultureEventBroker
{
    ValueTask RaiseCultureAddEventAsync(EventMessage<Culture> message);

    ValueTask RaiseCultureUpdateEventAsync(EventMessage<Culture> message);

    ValueTask RaiseCultureDeleteEventAsync(EventMessage<Culture> message);
}
