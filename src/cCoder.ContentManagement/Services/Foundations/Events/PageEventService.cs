// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class PageEventService(IPageEventBroker pageEventBroker) : IPageEventService
{
    public ValueTask RaisePageAddEventAsync(Page entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePageAddEventAsync(inputs: [entity]);
        ValidatePage(page: entity, parameterName: "entity");

        EventMessage<Page> message = new EventMessage<Page>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = pageEventBroker.GetCurrentUserId()
            },
            Data = entity
        };

        await pageEventBroker.RaisePageAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaisePageUpdateEventAsync(Page entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePageUpdateEventAsync(inputs: [entity]);
        ValidatePage(page: entity, parameterName: "entity");

        EventMessage<Page> message = new EventMessage<Page>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = pageEventBroker.GetCurrentUserId()
            },
            Data = entity
        };

        await pageEventBroker.RaisePageUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaisePageDeleteEventAsync(Page entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePageDeleteEventAsync(inputs: [entity]);
        ValidatePage(page: entity, parameterName: "entity");

        EventMessage<Page> message = new EventMessage<Page>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = pageEventBroker.GetCurrentUserId()
            },
            Data = entity
        };

        await pageEventBroker.RaisePageDeleteEventAsync(message: message);

    }, isValueTask: true);
}