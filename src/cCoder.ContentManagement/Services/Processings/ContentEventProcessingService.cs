// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class ContentEventProcessingService(IContentEventService eventService)
    : IContentEventProcessingService
{
    public ValueTask RaiseContentAddEventAsync(Content content, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseContentAddEventAsync(inputs: [content, userId]);
        ValidateContent(content: content, parameterName: "entity");


        return eventService.RaiseContentAddEventAsync(
            entity: content,
            userId: userId);

    }, isValueTask: true);

    public ValueTask RaiseContentUpdateEventAsync(Content content, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseContentUpdateEventAsync(inputs: [content, userId]);
        ValidateContent(content: content, parameterName: "entity");


        return eventService.RaiseContentUpdateEventAsync(
            entity: content,
            userId: userId);

    }, isValueTask: true);

    public ValueTask RaiseContentDeleteEventAsync(Content content, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseContentDeleteEventAsync(inputs: [content, userId]);
        ValidateContent(content: content, parameterName: "entity");


        return eventService.RaiseContentDeleteEventAsync(
            entity: content,
            userId: userId);

    }, isValueTask: true);

    private static void ValidateContent(Content content, string parameterName) =>
        ThrowIf(condition: content == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}