// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class ContentEventService(IContentEventBroker contentEventBroker) : IContentEventService
{
    public ValueTask RaiseContentAddEventAsync(Content content, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseContentAddEventAsync(inputs: [content, userId]);

        EventMessage<Content> message = new EventMessage<Content>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = content
        };

        await contentEventBroker.RaiseContentAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseContentUpdateEventAsync(Content content, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseContentUpdateEventAsync(inputs: [content, userId]);

        EventMessage<Content> message = new EventMessage<Content>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = content
        };

        await contentEventBroker.RaiseContentUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseContentDeleteEventAsync(Content content, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseContentDeleteEventAsync(inputs: [content, userId]);

        EventMessage<Content> message = new EventMessage<Content>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = content
        };

        await contentEventBroker.RaiseContentDeleteEventAsync(message: message);

    }, isValueTask: true);
}