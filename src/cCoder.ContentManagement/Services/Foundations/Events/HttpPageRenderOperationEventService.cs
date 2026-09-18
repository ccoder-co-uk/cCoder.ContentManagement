// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.ContentManagement.Models;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal sealed partial class HttpPageRenderOperationEventService(
    IRenderEventBroker renderEventBroker)
        : IHttpPageRenderOperationEventService
{
    public ValueTask RaiseHttpPageRenderOperationRenderRequestAsync(
        HttpPageRenderOperation httpPageRenderOperation,
        string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseHttpPageRenderOperationRenderRequestAsync(
            inputs: [httpPageRenderOperation, userId]);

        EventMessage<HttpPageRenderOperation> eventMessage = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = userId },
            Data = httpPageRenderOperation
        };

        await renderEventBroker
            .RaiseHttpPageRenderOperationRenderRequestAsync(
                message: eventMessage);
    }, isValueTask: true);
}