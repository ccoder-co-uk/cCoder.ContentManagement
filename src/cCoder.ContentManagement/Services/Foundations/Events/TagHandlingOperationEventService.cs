// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal sealed partial class TagHandlingOperationEventService(
    IRenderEventBroker renderEventBroker)
        : ITagHandlingOperationEventService
{
    public ValueTask RaiseTagHandlingOperationRenderTagsAsync(
        TagHandlingOperation tagHandlingOperation,
        string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseTagHandlingOperationRenderTagsAsync(
            inputs: [tagHandlingOperation, userId]);

        EventMessage<TagHandlingOperation> eventMessage = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = userId },
            Data = tagHandlingOperation
        };

        await renderEventBroker.RaiseTagHandlingOperationRenderTagsAsync(
            message: eventMessage);
    }, isValueTask: true);
}