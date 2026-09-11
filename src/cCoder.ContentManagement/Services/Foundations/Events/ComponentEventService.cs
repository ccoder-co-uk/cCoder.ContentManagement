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
    public ValueTask RaiseComponentAddEventAsync(Component component) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseComponentAddEventAsync(inputs: [component]);

        EventMessage<Component> message = new EventMessage<Component>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = componentEventBroker.GetCurrentUserId()
            },
            Data = component
        };

        await componentEventBroker.RaiseComponentAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseComponentUpdateEventAsync(Component component) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseComponentUpdateEventAsync(inputs: [component]);

        EventMessage<Component> message = new EventMessage<Component>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = componentEventBroker.GetCurrentUserId()
            },
            Data = component
        };

        await componentEventBroker.RaiseComponentUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseComponentDeleteEventAsync(Component component) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseComponentDeleteEventAsync(inputs: [component]);

        EventMessage<Component> message = new EventMessage<Component>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = componentEventBroker.GetCurrentUserId()
            },
            Data = component
        };

        await componentEventBroker.RaiseComponentDeleteEventAsync(message: message);

    }, isValueTask: true);
}