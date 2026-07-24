// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class ScriptEventService(IScriptEventBroker scriptEventBroker) : IScriptEventService
{
    public ValueTask RaiseScriptAddEventAsync(Script entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseScriptAddEventAsync(inputs: [entity]);

        EventMessage<Script> message = new EventMessage<Script>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = scriptEventBroker.GetCurrentUserId()
            },
            Data = entity
        };

        await scriptEventBroker.RaiseScriptAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseScriptUpdateEventAsync(Script entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseScriptUpdateEventAsync(inputs: [entity]);

        EventMessage<Script> message = new EventMessage<Script>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = scriptEventBroker.GetCurrentUserId()
            },
            Data = entity
        };

        await scriptEventBroker.RaiseScriptUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseScriptDeleteEventAsync(Script entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseScriptDeleteEventAsync(inputs: [entity]);

        EventMessage<Script> message = new EventMessage<Script>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = scriptEventBroker.GetCurrentUserId()
            },
            Data = entity
        };

        await scriptEventBroker.RaiseScriptDeleteEventAsync(message: message);

    }, isValueTask: true);
}