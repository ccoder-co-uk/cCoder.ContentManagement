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
    public ValueTask RaisePageInfoAddEventAsync(PageInfo pageInfo) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePageInfoAddEventAsync(inputs: [pageInfo]);

        EventMessage<PageInfo> message = new EventMessage<PageInfo>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = pageInfoEventBroker.GetCurrentUserId()
            },
            Data = pageInfo
        };

        await pageInfoEventBroker.RaisePageInfoAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaisePageInfoUpdateEventAsync(PageInfo pageInfo) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePageInfoUpdateEventAsync(inputs: [pageInfo]);

        EventMessage<PageInfo> message = new EventMessage<PageInfo>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = pageInfoEventBroker.GetCurrentUserId()
            },
            Data = pageInfo
        };

        await pageInfoEventBroker.RaisePageInfoUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaisePageInfoDeleteEventAsync(PageInfo pageInfo) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePageInfoDeleteEventAsync(inputs: [pageInfo]);

        EventMessage<PageInfo> message = new EventMessage<PageInfo>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = pageInfoEventBroker.GetCurrentUserId()
            },
            Data = pageInfo
        };

        await pageInfoEventBroker.RaisePageInfoDeleteEventAsync(message: message);

    }, isValueTask: true);
}