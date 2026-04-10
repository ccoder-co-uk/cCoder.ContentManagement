using cCoder.Data.Models.CMS;
using EventLibrary.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public interface IScriptEventBroker
{
    ValueTask RaiseScriptAddEventAsync(EventMessage<Script> message);

    ValueTask RaiseScriptUpdateEventAsync(EventMessage<Script> message);

    ValueTask RaiseScriptDeleteEventAsync(EventMessage<Script> message);
}
