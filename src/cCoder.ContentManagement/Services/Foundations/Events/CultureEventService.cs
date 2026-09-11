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
    public ValueTask RaiseCultureAddEventAsync(Culture culture) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseCultureAddEventAsync(inputs: [culture]);

        EventMessage<Culture> message = new EventMessage<Culture>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = cultureEventBroker.GetCurrentUserId()
            },
            Data = culture
        };

        await cultureEventBroker.RaiseCultureAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseCultureUpdateEventAsync(Culture culture) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseCultureUpdateEventAsync(inputs: [culture]);

        EventMessage<Culture> message = new EventMessage<Culture>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = cultureEventBroker.GetCurrentUserId()
            },
            Data = culture
        };

        await cultureEventBroker.RaiseCultureUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseCultureDeleteEventAsync(Culture culture) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseCultureDeleteEventAsync(inputs: [culture]);

        EventMessage<Culture> message = new EventMessage<Culture>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = cultureEventBroker.GetCurrentUserId()
            },
            Data = culture
        };

        await cultureEventBroker.RaiseCultureDeleteEventAsync(message: message);

    }, isValueTask: true);
}