// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal class CommonObjectEventService(ICommonObjectEventBroker commonObjectEventBroker, ICoreAuthInfo authInfo) : ICommonObjectEventService
{
    public async ValueTask RaiseCommonObjectAddEventAsync(CommonObject entity)
    {
        EventMessage<CommonObject> message = new EventMessage<CommonObject>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await commonObjectEventBroker.RaiseCommonObjectAddEventAsync(message: message);
    }

    public async ValueTask RaiseCommonObjectUpdateEventAsync(CommonObject entity)
    {
        EventMessage<CommonObject> message = new EventMessage<CommonObject>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await commonObjectEventBroker.RaiseCommonObjectUpdateEventAsync(message: message);
    }

    public async ValueTask RaiseCommonObjectDeleteEventAsync(CommonObject entity)
    {
        EventMessage<CommonObject> message = new EventMessage<CommonObject>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await commonObjectEventBroker.RaiseCommonObjectDeleteEventAsync(message: message);
    }
}