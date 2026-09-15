// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class LayoutEventService(ILayoutEventBroker layoutEventBroker) : ILayoutEventService
{
    public ValueTask RaiseLayoutAddEventAsync(Layout layout, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseLayoutAddEventAsync(inputs: [layout, userId]);

        EventMessage<Layout> message = new EventMessage<Layout>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = layout
        };

        await layoutEventBroker.RaiseLayoutAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseLayoutUpdateEventAsync(Layout layout, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseLayoutUpdateEventAsync(inputs: [layout, userId]);

        EventMessage<Layout> message = new EventMessage<Layout>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = layout
        };

        await layoutEventBroker.RaiseLayoutUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseLayoutDeleteEventAsync(Layout layout, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseLayoutDeleteEventAsync(inputs: [layout, userId]);

        EventMessage<Layout> message = new EventMessage<Layout>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = layout
        };

        await layoutEventBroker.RaiseLayoutDeleteEventAsync(message: message);

    }, isValueTask: true);
}