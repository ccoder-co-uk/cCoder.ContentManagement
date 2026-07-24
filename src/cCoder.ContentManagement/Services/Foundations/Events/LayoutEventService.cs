// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class LayoutEventService(ILayoutEventBroker layoutEventBroker, ICoreAuthInfo authInfo) : ILayoutEventService
{
    public async ValueTask RaiseLayoutAddEventAsync(Layout entity)
    {
        EventMessage<Layout> message = new EventMessage<Layout>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await layoutEventBroker.RaiseLayoutAddEventAsync(message: message);
    }

    public async ValueTask RaiseLayoutUpdateEventAsync(Layout entity)
    {
        EventMessage<Layout> message = new EventMessage<Layout>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await layoutEventBroker.RaiseLayoutUpdateEventAsync(message: message);
    }

    public async ValueTask RaiseLayoutDeleteEventAsync(Layout entity)
    {
        EventMessage<Layout> message = new EventMessage<Layout>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await layoutEventBroker.RaiseLayoutDeleteEventAsync(message: message);
    }
}