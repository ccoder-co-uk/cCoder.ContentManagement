using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using EventLibrary.Models;
using Page = cCoder.Data.Models.CMS.Page;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class PageEventService(IPageEventBroker pageEventBroker, ICoreAuthInfo authInfo) : IPageEventService
{
    public async ValueTask RaisePageAddEventAsync(Page entity)
    {
        ValidatePage(entity, "entity");
        EventMessage<Page> message = new EventMessage<Page>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };
        await pageEventBroker.RaisePageAddEventAsync(message);
    }

    public async ValueTask RaisePageUpdateEventAsync(Page entity)
    {
        ValidatePage(entity, "entity");
        EventMessage<Page> message = new EventMessage<Page>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };
        await pageEventBroker.RaisePageUpdateEventAsync(message);
    }

    public async ValueTask RaisePageDeleteEventAsync(Page entity)
    {
        ValidatePage(entity, "entity");
        EventMessage<Page> message = new EventMessage<Page>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };
        await pageEventBroker.RaisePageDeleteEventAsync(message);
    }
}
