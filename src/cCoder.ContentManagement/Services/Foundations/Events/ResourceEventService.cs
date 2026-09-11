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
    public ValueTask RaiseResourceAddEventAsync(Resource resource) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseResourceAddEventAsync(inputs: [resource]);

        EventMessage<Resource> message = new EventMessage<Resource>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = resourceEventBroker.GetCurrentUserId()
            },
            Data = resource
        };

        await resourceEventBroker.RaiseResourceAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseResourceUpdateEventAsync(Resource resource) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseResourceUpdateEventAsync(inputs: [resource]);

        EventMessage<Resource> message = new EventMessage<Resource>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = resourceEventBroker.GetCurrentUserId()
            },
            Data = resource
        };

        await resourceEventBroker.RaiseResourceUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseResourceDeleteEventAsync(Resource resource) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseResourceDeleteEventAsync(inputs: [resource]);

        EventMessage<Resource> message = new EventMessage<Resource>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = resourceEventBroker.GetCurrentUserId()
            },
            Data = resource
        };

        await resourceEventBroker.RaiseResourceDeleteEventAsync(message: message);

    }, isValueTask: true);
}