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
    public ValueTask RaiseLayoutAddEventAsync(Layout entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseLayoutAddEventAsync(inputs: [entity]);

        EventMessage<Layout> message = new EventMessage<Layout>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = layoutEventBroker.GetCurrentUserId()
            },
            Data = entity
        };

        await layoutEventBroker.RaiseLayoutAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseLayoutUpdateEventAsync(Layout entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseLayoutUpdateEventAsync(inputs: [entity]);

        EventMessage<Layout> message = new EventMessage<Layout>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = layoutEventBroker.GetCurrentUserId()
            },
            Data = entity
        };

        await layoutEventBroker.RaiseLayoutUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseLayoutDeleteEventAsync(Layout entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseLayoutDeleteEventAsync(inputs: [entity]);

        EventMessage<Layout> message = new EventMessage<Layout>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = layoutEventBroker.GetCurrentUserId()
            },
            Data = entity
        };

        await layoutEventBroker.RaiseLayoutDeleteEventAsync(message: message);

    }, isValueTask: true);
}