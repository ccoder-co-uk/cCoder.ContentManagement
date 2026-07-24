// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal class PageInfoEventService(IPageInfoEventBroker pageInfoEventBroker, ICoreAuthInfo authInfo) : IPageInfoEventService
{
    public async ValueTask RaisePageInfoAddEventAsync(PageInfo entity)
    {
        EventMessage<PageInfo> message = new EventMessage<PageInfo>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await pageInfoEventBroker.RaisePageInfoAddEventAsync(message: message);
    }

    public async ValueTask RaisePageInfoUpdateEventAsync(PageInfo entity)
    {
        EventMessage<PageInfo> message = new EventMessage<PageInfo>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await pageInfoEventBroker.RaisePageInfoUpdateEventAsync(message: message);
    }

    public async ValueTask RaisePageInfoDeleteEventAsync(PageInfo entity)
    {
        EventMessage<PageInfo> message = new EventMessage<PageInfo>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await pageInfoEventBroker.RaisePageInfoDeleteEventAsync(message: message);
    }
}