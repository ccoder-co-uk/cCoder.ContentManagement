using cCoder.Data.Models.CMS;
using EventLibrary.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public interface ITemplateEventBroker
{
    ValueTask RaiseTemplateAddEventAsync(EventMessage<Template> message);

    ValueTask RaiseTemplateUpdateEventAsync(EventMessage<Template> message);

    ValueTask RaiseTemplateDeleteEventAsync(EventMessage<Template> message);
}
