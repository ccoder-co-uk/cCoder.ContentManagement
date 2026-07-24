// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class ContentEventProcessingService(IContentEventService eventService) : IContentEventProcessingService
{
    public ValueTask RaiseContentAddEventAsync(Content entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseContentAddEventAsync(inputs: [entity]);
        ValidateContent(content: entity, parameterName: "entity");

        return eventService.RaiseContentAddEventAsync(entity: entity);

    }, isValueTask: true);

    public ValueTask RaiseContentUpdateEventAsync(Content entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseContentUpdateEventAsync(inputs: [entity]);
        ValidateContent(content: entity, parameterName: "entity");

        return eventService.RaiseContentUpdateEventAsync(entity: entity);

    }, isValueTask: true);

    public ValueTask RaiseContentDeleteEventAsync(Content entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseContentDeleteEventAsync(inputs: [entity]);
        ValidateContent(content: entity, parameterName: "entity");

        return eventService.RaiseContentDeleteEventAsync(entity: entity);

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