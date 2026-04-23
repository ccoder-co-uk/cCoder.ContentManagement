using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using Resource = cCoder.Data.Models.CMS.Resource;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class ResourceEventService(IResourceEventBroker resourceEventBroker, ICoreAuthInfo authInfo) : IResourceEventService
{
    public async ValueTask RaiseResourceAddEventAsync(Resource entity)
    {
        EventMessage<Resource> message = new EventMessage<Resource>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };
        await resourceEventBroker.RaiseResourceAddEventAsync(message);
    }

    public async ValueTask RaiseResourceUpdateEventAsync(Resource entity)
    {
        EventMessage<Resource> message = new EventMessage<Resource>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };
        await resourceEventBroker.RaiseResourceUpdateEventAsync(message);
    }

    public async ValueTask RaiseResourceDeleteEventAsync(Resource entity)
    {
        EventMessage<Resource> message = new EventMessage<Resource>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };
        await resourceEventBroker.RaiseResourceDeleteEventAsync(message);
    }
}
