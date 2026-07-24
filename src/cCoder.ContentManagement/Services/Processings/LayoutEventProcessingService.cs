// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal class LayoutEventProcessingService(ILayoutEventService eventService) : ILayoutEventProcessingService
{
    public ValueTask RaiseLayoutAddEventAsync(Layout entity)
    {
        ValidateLayout(layout: entity, parameterName: "entity");

        return eventService.RaiseLayoutAddEventAsync(entity: entity);
    }

    public ValueTask RaiseLayoutUpdateEventAsync(Layout entity)
    {
        ValidateLayout(layout: entity, parameterName: "entity");

        return eventService.RaiseLayoutUpdateEventAsync(entity: entity);
    }

    public ValueTask RaiseLayoutDeleteEventAsync(Layout entity)
    {
        ValidateLayout(layout: entity, parameterName: "entity");

        return eventService.RaiseLayoutDeleteEventAsync(entity: entity);
    }

    private static void ValidateLayout(Layout layout, string parameterName) =>
        ThrowIf(condition: layout == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}