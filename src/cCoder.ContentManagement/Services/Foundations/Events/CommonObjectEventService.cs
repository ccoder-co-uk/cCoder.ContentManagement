// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class CommonObjectEventService(ICommonObjectEventBroker commonObjectEventBroker, ICoreAuthInfo authInfo) : ICommonObjectEventService
{
    public ValueTask RaiseCommonObjectAddEventAsync(CommonObject entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseCommonObjectAddEventAsync(inputs: [entity]);

        EventMessage<CommonObject> message = new EventMessage<CommonObject>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await commonObjectEventBroker.RaiseCommonObjectAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseCommonObjectUpdateEventAsync(CommonObject entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseCommonObjectUpdateEventAsync(inputs: [entity]);

        EventMessage<CommonObject> message = new EventMessage<CommonObject>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await commonObjectEventBroker.RaiseCommonObjectUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseCommonObjectDeleteEventAsync(CommonObject entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseCommonObjectDeleteEventAsync(inputs: [entity]);

        EventMessage<CommonObject> message = new EventMessage<CommonObject>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await commonObjectEventBroker.RaiseCommonObjectDeleteEventAsync(message: message);

    }, isValueTask: true);
}