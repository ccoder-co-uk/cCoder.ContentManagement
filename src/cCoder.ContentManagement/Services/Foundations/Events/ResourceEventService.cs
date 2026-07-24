// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class ResourceEventService(IResourceEventBroker resourceEventBroker) : IResourceEventService
{
    public ValueTask RaiseResourceAddEventAsync(Resource entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseResourceAddEventAsync(inputs: [entity]);

        EventMessage<Resource> message = new EventMessage<Resource>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = resourceEventBroker.GetCurrentUserId()
            },
            Data = entity
        };

        await resourceEventBroker.RaiseResourceAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseResourceUpdateEventAsync(Resource entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseResourceUpdateEventAsync(inputs: [entity]);

        EventMessage<Resource> message = new EventMessage<Resource>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = resourceEventBroker.GetCurrentUserId()
            },
            Data = entity
        };

        await resourceEventBroker.RaiseResourceUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseResourceDeleteEventAsync(Resource entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseResourceDeleteEventAsync(inputs: [entity]);

        EventMessage<Resource> message = new EventMessage<Resource>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = resourceEventBroker.GetCurrentUserId()
            },
            Data = entity
        };

        await resourceEventBroker.RaiseResourceDeleteEventAsync(message: message);

    }, isValueTask: true);
}