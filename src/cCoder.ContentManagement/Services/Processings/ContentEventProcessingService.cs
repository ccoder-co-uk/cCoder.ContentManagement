// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal class ContentEventProcessingService(IContentEventService eventService) : IContentEventProcessingService
{
    public ValueTask RaiseContentAddEventAsync(Content entity)
    {
        ValidateContent(content: entity, parameterName: "entity");

        return eventService.RaiseContentAddEventAsync(entity: entity);
    }

    public ValueTask RaiseContentUpdateEventAsync(Content entity)
    {
        ValidateContent(content: entity, parameterName: "entity");

        return eventService.RaiseContentUpdateEventAsync(entity: entity);
    }

    public ValueTask RaiseContentDeleteEventAsync(Content entity)
    {
        ValidateContent(content: entity, parameterName: "entity");

        return eventService.RaiseContentDeleteEventAsync(entity: entity);
    }

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