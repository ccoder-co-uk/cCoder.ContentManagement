// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class PageInfoEventService(IPageInfoEventBroker pageInfoEventBroker) : IPageInfoEventService
{
    public ValueTask RaisePageInfoAddEventAsync(PageInfo entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePageInfoAddEventAsync(inputs: [entity]);

        EventMessage<PageInfo> message = new EventMessage<PageInfo>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = pageInfoEventBroker.GetCurrentUserId()
            },
            Data = entity
        };

        await pageInfoEventBroker.RaisePageInfoAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaisePageInfoUpdateEventAsync(PageInfo entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePageInfoUpdateEventAsync(inputs: [entity]);

        EventMessage<PageInfo> message = new EventMessage<PageInfo>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = pageInfoEventBroker.GetCurrentUserId()
            },
            Data = entity
        };

        await pageInfoEventBroker.RaisePageInfoUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaisePageInfoDeleteEventAsync(PageInfo entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePageInfoDeleteEventAsync(inputs: [entity]);

        EventMessage<PageInfo> message = new EventMessage<PageInfo>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = pageInfoEventBroker.GetCurrentUserId()
            },
            Data = entity
        };

        await pageInfoEventBroker.RaisePageInfoDeleteEventAsync(message: message);

    }, isValueTask: true);
}