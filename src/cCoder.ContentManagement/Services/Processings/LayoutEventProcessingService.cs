// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class LayoutEventProcessingService(ILayoutEventService eventService) : ILayoutEventProcessingService
{
    public ValueTask RaiseLayoutAddEventAsync(Layout layout) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseLayoutAddEventAsync(inputs: [layout]);
        ValidateLayout(layout: layout, parameterName: "entity");

        return eventService.RaiseLayoutAddEventAsync(entity: layout);

    }, isValueTask: true);

    public ValueTask RaiseLayoutUpdateEventAsync(Layout layout) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseLayoutUpdateEventAsync(inputs: [layout]);
        ValidateLayout(layout: layout, parameterName: "entity");

        return eventService.RaiseLayoutUpdateEventAsync(entity: layout);

    }, isValueTask: true);

    public ValueTask RaiseLayoutDeleteEventAsync(Layout layout) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseLayoutDeleteEventAsync(inputs: [layout]);
        ValidateLayout(layout: layout, parameterName: "entity");

        return eventService.RaiseLayoutDeleteEventAsync(entity: layout);

    }, isValueTask: true);

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