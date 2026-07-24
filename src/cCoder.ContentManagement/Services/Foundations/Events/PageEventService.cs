// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class PageEventService(IPageEventBroker pageEventBroker, ICoreAuthInfo authInfo) : IPageEventService
{
    public async ValueTask RaisePageAddEventAsync(Page entity)
    {
        ValidatePage(page: entity, parameterName: "entity");

        EventMessage<Page> message = new EventMessage<Page>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await pageEventBroker.RaisePageAddEventAsync(message: message);
    }

    public async ValueTask RaisePageUpdateEventAsync(Page entity)
    {
        ValidatePage(page: entity, parameterName: "entity");

        EventMessage<Page> message = new EventMessage<Page>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await pageEventBroker.RaisePageUpdateEventAsync(message: message);
    }

    public async ValueTask RaisePageDeleteEventAsync(Page entity)
    {
        ValidatePage(page: entity, parameterName: "entity");

        EventMessage<Page> message = new EventMessage<Page>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await pageEventBroker.RaisePageDeleteEventAsync(message: message);
    }
}