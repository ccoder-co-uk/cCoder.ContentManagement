using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using PageRole = cCoder.Data.Models.Security.PageRole;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal class PageRoleEventService(IPageRoleEventBroker pageRoleEventBroker, ICoreAuthInfo authInfo) : IPageRoleEventService
{
    public async ValueTask RaisePageRoleAddEventAsync(PageRole entity)
    {
        EventMessage<PageRole> message = new EventMessage<PageRole>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };
        await pageRoleEventBroker.RaisePageRoleAddEventAsync(message);
    }

    public async ValueTask RaisePageRoleDeleteEventAsync(PageRole entity)
    {
        EventMessage<PageRole> message = new EventMessage<PageRole>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };
        await pageRoleEventBroker.RaisePageRoleDeleteEventAsync(message);
    }
}
