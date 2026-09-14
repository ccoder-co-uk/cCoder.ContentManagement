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
    public ValueTask RaiseComponentAddEventAsync(Component component, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseComponentAddEventAsync(inputs: [component, userId]);

        EventMessage<Component> message = new EventMessage<Component>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = component
        };

        await componentEventBroker.RaiseComponentAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseComponentUpdateEventAsync(Component component, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseComponentUpdateEventAsync(inputs: [component, userId]);

        EventMessage<Component> message = new EventMessage<Component>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = component
        };

        await componentEventBroker.RaiseComponentUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseComponentDeleteEventAsync(Component component, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseComponentDeleteEventAsync(inputs: [component, userId]);

        EventMessage<Component> message = new EventMessage<Component>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = component
        };

        await componentEventBroker.RaiseComponentDeleteEventAsync(message: message);

    }, isValueTask: true);
}