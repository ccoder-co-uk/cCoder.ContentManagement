// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

internal sealed class ScriptEventBroker(IEventInfrastructureDependency eventInfrastructureDependency)
    : AuthenticatedEventBroker(eventInfrastructureDependency), IScriptEventBroker
{
    public ValueTask RaiseScriptAddEventAsync(EventMessage<Script> message) =>
        eventInfrastructureDependency.RaiseEventAsync(name: "script_add", message: message);

    public ValueTask RaiseScriptUpdateEventAsync(EventMessage<Script> message) =>
        eventInfrastructureDependency.RaiseEventAsync(name: "script_update", message: message);

    public ValueTask RaiseScriptDeleteEventAsync(EventMessage<Script> message) =>
        eventInfrastructureDependency.RaiseEventAsync(name: "script_delete", message: message);
}