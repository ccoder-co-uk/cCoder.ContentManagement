using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using EventLibrary.Models;
using Layout = cCoder.Data.Models.CMS.Layout;

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
        await layoutEventBroker.RaiseLayoutAddEventAsync(message);
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
        await layoutEventBroker.RaiseLayoutUpdateEventAsync(message);
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
        await layoutEventBroker.RaiseLayoutDeleteEventAsync(message);
    }
}
