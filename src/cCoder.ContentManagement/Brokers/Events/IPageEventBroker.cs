using cCoder.Data.Models.CMS;
using EventLibrary.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public interface IPageEventBroker
{
    ValueTask RaisePageAddEventAsync(EventMessage<Page> message);

    ValueTask RaisePageUpdateEventAsync(EventMessage<Page> message);

    ValueTask RaisePageDeleteEventAsync(EventMessage<Page> message);
}
