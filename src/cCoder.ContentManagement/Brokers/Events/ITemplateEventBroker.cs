using cCoder.Data.Models.CMS;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public interface ITemplateEventBroker
{
    ValueTask RaiseTemplateAddEventAsync(EventMessage<Template> message);

    ValueTask RaiseTemplateUpdateEventAsync(EventMessage<Template> message);

    ValueTask RaiseTemplateDeleteEventAsync(EventMessage<Template> message);
}
