using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using EventLibrary.Models;
using Content = cCoder.Data.Models.CMS.Content;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class ContentEventService(IContentEventBroker contentEventBroker, ICoreAuthInfo authInfo) : IContentEventService
{
    public async ValueTask RaiseContentAddEventAsync(Content entity)
    {
        EventMessage<Content> message = new EventMessage<Content>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };
        await contentEventBroker.RaiseContentAddEventAsync(message);
    }

    public async ValueTask RaiseContentUpdateEventAsync(Content entity)
    {
        EventMessage<Content> message = new EventMessage<Content>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };
        await contentEventBroker.RaiseContentUpdateEventAsync(message);
    }

    public async ValueTask RaiseContentDeleteEventAsync(Content entity)
    {
        EventMessage<Content> message = new EventMessage<Content>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };
        await contentEventBroker.RaiseContentDeleteEventAsync(message);
    }
}
