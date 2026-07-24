// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class ContentEventService(IContentEventBroker contentEventBroker, ICoreAuthInfo authInfo) : IContentEventService
{
    public ValueTask RaiseContentAddEventAsync(Content entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseContentAddEventAsync(inputs: [entity]);

        EventMessage<Content> message = new EventMessage<Content>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await contentEventBroker.RaiseContentAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseContentUpdateEventAsync(Content entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseContentUpdateEventAsync(inputs: [entity]);

        EventMessage<Content> message = new EventMessage<Content>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await contentEventBroker.RaiseContentUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseContentDeleteEventAsync(Content entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseContentDeleteEventAsync(inputs: [entity]);

        EventMessage<Content> message = new EventMessage<Content>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await contentEventBroker.RaiseContentDeleteEventAsync(message: message);

    }, isValueTask: true);
}