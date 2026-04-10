using cCoder.Data.Models.CMS;
using EventLibrary;
using EventLibrary.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public class ScriptEventBroker(IEventHub eventHub) : IScriptEventBroker
{
    public ValueTask RaiseScriptAddEventAsync(EventMessage<Script> message)
    {
        return eventHub.RaiseEventAsync("script_add", message);
    }

    public ValueTask RaiseScriptUpdateEventAsync(EventMessage<Script> message)
    {
        return eventHub.RaiseEventAsync("script_update", message);
    }

    public ValueTask RaiseScriptDeleteEventAsync(EventMessage<Script> message)
    {
        return eventHub.RaiseEventAsync("script_delete", message);
    }
}
