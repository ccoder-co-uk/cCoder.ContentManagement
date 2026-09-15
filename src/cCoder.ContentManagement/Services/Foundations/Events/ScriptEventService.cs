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
    public ValueTask RaiseScriptAddEventAsync(Script script, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseScriptAddEventAsync(inputs: [script, userId]);

        EventMessage<Script> message = new EventMessage<Script>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = script
        };

        await scriptEventBroker.RaiseScriptAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseScriptUpdateEventAsync(Script script, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseScriptUpdateEventAsync(inputs: [script, userId]);

        EventMessage<Script> message = new EventMessage<Script>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = script
        };

        await scriptEventBroker.RaiseScriptUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseScriptDeleteEventAsync(Script script, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseScriptDeleteEventAsync(inputs: [script, userId]);

        EventMessage<Script> message = new EventMessage<Script>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = script
        };

        await scriptEventBroker.RaiseScriptDeleteEventAsync(message: message);

    }, isValueTask: true);
}