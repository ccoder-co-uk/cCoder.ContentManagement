// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class CultureEventService(ICultureEventBroker cultureEventBroker) : ICultureEventService
{
    public ValueTask RaiseCultureAddEventAsync(Culture entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseCultureAddEventAsync(inputs: [entity]);

        EventMessage<Culture> message = new EventMessage<Culture>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = cultureEventBroker.GetCurrentUserId()
            },
            Data = entity
        };

        await cultureEventBroker.RaiseCultureAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseCultureUpdateEventAsync(Culture entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseCultureUpdateEventAsync(inputs: [entity]);

        EventMessage<Culture> message = new EventMessage<Culture>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = cultureEventBroker.GetCurrentUserId()
            },
            Data = entity
        };

        await cultureEventBroker.RaiseCultureUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseCultureDeleteEventAsync(Culture entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseCultureDeleteEventAsync(inputs: [entity]);

        EventMessage<Culture> message = new EventMessage<Culture>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = cultureEventBroker.GetCurrentUserId()
            },
            Data = entity
        };

        await cultureEventBroker.RaiseCultureDeleteEventAsync(message: message);

    }, isValueTask: true);
}