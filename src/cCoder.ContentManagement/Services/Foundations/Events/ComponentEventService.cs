// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using cCoder.Data.Models.CMS;

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

        await componentEventBroker.RaiseComponentAddEventAsync(message: message);
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

        await componentEventBroker.RaiseComponentUpdateEventAsync(message: message);
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

        await componentEventBroker.RaiseComponentDeleteEventAsync(message: message);
    }
}