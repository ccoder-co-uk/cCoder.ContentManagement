using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using Component = cCoder.Data.Models.CMS.Component;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class ComponentEventService(IComponentEventBroker componentEventBroker, ICoreAuthInfo authInfo) : IComponentEventService
{
    public async ValueTask RaiseComponentAddEventAsync(Component entity)
    {
        EventMessage<Component> message = new EventMessage<Component>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };
        await componentEventBroker.RaiseComponentAddEventAsync(message);
    }

    public async ValueTask RaiseComponentUpdateEventAsync(Component entity)
    {
        EventMessage<Component> message = new EventMessage<Component>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };
        await componentEventBroker.RaiseComponentUpdateEventAsync(message);
    }

    public async ValueTask RaiseComponentDeleteEventAsync(Component entity)
    {
        EventMessage<Component> message = new EventMessage<Component>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };
        await componentEventBroker.RaiseComponentDeleteEventAsync(message);
    }
}
