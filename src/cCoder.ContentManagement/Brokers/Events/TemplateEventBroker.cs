using cCoder.Data.Models.CMS;
using EventLibrary;
using EventLibrary.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public class TemplateEventBroker(IEventHub eventHub) : ITemplateEventBroker
{
    public ValueTask RaiseTemplateAddEventAsync(EventMessage<Template> message)
    {
        return eventHub.RaiseEventAsync("template_add", message);
    }

    public ValueTask RaiseTemplateUpdateEventAsync(EventMessage<Template> message)
    {
        return eventHub.RaiseEventAsync("template_update", message);
    }

    public ValueTask RaiseTemplateDeleteEventAsync(EventMessage<Template> message)
    {
        return eventHub.RaiseEventAsync("template_delete", message);
    }
}
