// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Services.Foundations.Events;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class TagHandlingOperationEventProcessingService(
    ITagHandlingOperationEventService eventService)
        : ITagHandlingOperationEventProcessingService
{
    public ValueTask RaiseTagHandlingOperationRenderTagsAsync(
        TagHandlingOperation tagHandlingOperation,
        string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseTagHandlingOperationRenderTagsAsync(
            inputs: [tagHandlingOperation, userId]);

        ThrowIf(
            condition: tagHandlingOperation is null,
            message: "The tag handling operation is required.");

        return eventService.RaiseTagHandlingOperationRenderTagsAsync(
            tagHandlingOperation: tagHandlingOperation,
            userId: userId);
    }, isValueTask: true);

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}