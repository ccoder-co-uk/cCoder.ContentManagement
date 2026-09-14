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
    public ValueTask RaiseCultureAddEventAsync(Culture culture, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseCultureAddEventAsync(inputs: [culture, userId]);

        EventMessage<Culture> message = new EventMessage<Culture>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = culture
        };

        await cultureEventBroker.RaiseCultureAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseCultureUpdateEventAsync(Culture culture, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseCultureUpdateEventAsync(inputs: [culture, userId]);

        EventMessage<Culture> message = new EventMessage<Culture>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = culture
        };

        await cultureEventBroker.RaiseCultureUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseCultureDeleteEventAsync(Culture culture, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseCultureDeleteEventAsync(inputs: [culture, userId]);

        EventMessage<Culture> message = new EventMessage<Culture>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = culture
        };

        await cultureEventBroker.RaiseCultureDeleteEventAsync(message: message);

    }, isValueTask: true);
}