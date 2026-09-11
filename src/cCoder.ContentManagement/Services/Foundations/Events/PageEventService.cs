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
    public ValueTask RaisePageAddEventAsync(Page page) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePageAddEventAsync(inputs: [page]);
        ValidatePage(page: page, parameterName: "entity");

        EventMessage<Page> message = new EventMessage<Page>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = pageEventBroker.GetCurrentUserId()
            },
            Data = page
        };

        await pageEventBroker.RaisePageAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaisePageUpdateEventAsync(Page page) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePageUpdateEventAsync(inputs: [page]);
        ValidatePage(page: page, parameterName: "entity");

        EventMessage<Page> message = new EventMessage<Page>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = pageEventBroker.GetCurrentUserId()
            },
            Data = page
        };

        await pageEventBroker.RaisePageUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaisePageDeleteEventAsync(Page page) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePageDeleteEventAsync(inputs: [page]);
        ValidatePage(page: page, parameterName: "entity");

        EventMessage<Page> message = new EventMessage<Page>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = pageEventBroker.GetCurrentUserId()
            },
            Data = page
        };

        await pageEventBroker.RaisePageDeleteEventAsync(message: message);

    }, isValueTask: true);

}