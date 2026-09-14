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
    public ValueTask RaisePageAddEventAsync(Page page, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePageAddEventAsync(inputs: [page, userId]);
        ValidatePage(page: page, parameterName: "entity");

        EventMessage<Page> message = new EventMessage<Page>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = page
        };

        await pageEventBroker.RaisePageAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaisePageUpdateEventAsync(Page page, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePageUpdateEventAsync(inputs: [page, userId]);
        ValidatePage(page: page, parameterName: "entity");

        EventMessage<Page> message = new EventMessage<Page>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = page
        };

        await pageEventBroker.RaisePageUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaisePageDeleteEventAsync(Page page, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePageDeleteEventAsync(inputs: [page, userId]);
        ValidatePage(page: page, parameterName: "entity");

        EventMessage<Page> message = new EventMessage<Page>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = page
        };

        await pageEventBroker.RaisePageDeleteEventAsync(message: message);

    }, isValueTask: true);

}