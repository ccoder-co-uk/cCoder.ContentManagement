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
    public ValueTask RaiseResourceAddEventAsync(Resource resource, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseResourceAddEventAsync(inputs: [resource, userId]);

        EventMessage<Resource> message = new EventMessage<Resource>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = resource
        };

        await resourceEventBroker.RaiseResourceAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseResourceUpdateEventAsync(Resource resource, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseResourceUpdateEventAsync(inputs: [resource, userId]);

        EventMessage<Resource> message = new EventMessage<Resource>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = resource
        };

        await resourceEventBroker.RaiseResourceUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseResourceDeleteEventAsync(Resource resource, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseResourceDeleteEventAsync(inputs: [resource, userId]);

        EventMessage<Resource> message = new EventMessage<Resource>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = resource
        };

        await resourceEventBroker.RaiseResourceDeleteEventAsync(message: message);

    }, isValueTask: true);
}