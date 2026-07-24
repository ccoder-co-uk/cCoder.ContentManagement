// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class ComponentEventService(IComponentEventBroker componentEventBroker) : IComponentEventService
{
    public ValueTask RaiseComponentAddEventAsync(Component entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseComponentAddEventAsync(inputs: [entity]);

        EventMessage<Component> message = new EventMessage<Component>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = componentEventBroker.GetCurrentUserId()
            },
            Data = entity
        };

        await componentEventBroker.RaiseComponentAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseComponentUpdateEventAsync(Component entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseComponentUpdateEventAsync(inputs: [entity]);

        EventMessage<Component> message = new EventMessage<Component>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = componentEventBroker.GetCurrentUserId()
            },
            Data = entity
        };

        await componentEventBroker.RaiseComponentUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseComponentDeleteEventAsync(Component entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseComponentDeleteEventAsync(inputs: [entity]);

        EventMessage<Component> message = new EventMessage<Component>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = componentEventBroker.GetCurrentUserId()
            },
            Data = entity
        };

        await componentEventBroker.RaiseComponentDeleteEventAsync(message: message);

    }, isValueTask: true);
}