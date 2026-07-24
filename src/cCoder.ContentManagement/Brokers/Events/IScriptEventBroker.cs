// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public interface IScriptEventBroker : IAuthenticatedEventBroker
{
    ValueTask RaiseScriptAddEventAsync(EventMessage<Script> message);

    ValueTask RaiseScriptUpdateEventAsync(EventMessage<Script> message);

    ValueTask RaiseScriptDeleteEventAsync(EventMessage<Script> message);
}