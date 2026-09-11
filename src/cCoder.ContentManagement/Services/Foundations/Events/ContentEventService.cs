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
    public ValueTask RaiseContentAddEventAsync(Content content) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseContentAddEventAsync(inputs: [content]);

        EventMessage<Content> message = new EventMessage<Content>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = contentEventBroker.GetCurrentUserId()
            },
            Data = content
        };

        await contentEventBroker.RaiseContentAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseContentUpdateEventAsync(Content content) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseContentUpdateEventAsync(inputs: [content]);

        EventMessage<Content> message = new EventMessage<Content>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = contentEventBroker.GetCurrentUserId()
            },
            Data = content
        };

        await contentEventBroker.RaiseContentUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseContentDeleteEventAsync(Content content) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseContentDeleteEventAsync(inputs: [content]);

        EventMessage<Content> message = new EventMessage<Content>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = contentEventBroker.GetCurrentUserId()
            },
            Data = content
        };

        await contentEventBroker.RaiseContentDeleteEventAsync(message: message);

    }, isValueTask: true);
}