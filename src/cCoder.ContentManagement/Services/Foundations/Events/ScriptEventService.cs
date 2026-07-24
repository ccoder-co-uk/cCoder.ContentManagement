// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class ScriptEventService(IScriptEventBroker scriptEventBroker, ICoreAuthInfo authInfo) : IScriptEventService
{
    public async ValueTask RaiseScriptAddEventAsync(Script entity)
    {
        EventMessage<Script> message = new EventMessage<Script>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await scriptEventBroker.RaiseScriptAddEventAsync(message: message);
    }

    public async ValueTask RaiseScriptUpdateEventAsync(Script entity)
    {
        EventMessage<Script> message = new EventMessage<Script>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await scriptEventBroker.RaiseScriptUpdateEventAsync(message: message);
    }

    public async ValueTask RaiseScriptDeleteEventAsync(Script entity)
    {
        EventMessage<Script> message = new EventMessage<Script>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await scriptEventBroker.RaiseScriptDeleteEventAsync(message: message);
    }
}