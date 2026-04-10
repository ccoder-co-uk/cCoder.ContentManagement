using cCoder.Data.Models.CMS;
using EventLibrary.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public interface IContentEventBroker
{
    ValueTask RaiseContentAddEventAsync(EventMessage<Content> message);

    ValueTask RaiseContentUpdateEventAsync(EventMessage<Content> message);

    ValueTask RaiseContentDeleteEventAsync(EventMessage<Content> message);
}
