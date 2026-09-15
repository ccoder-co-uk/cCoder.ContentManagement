// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class LayoutEventProcessingService(ILayoutEventService eventService)
    : ILayoutEventProcessingService
{
    public ValueTask RaiseLayoutAddEventAsync(Layout layout, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseLayoutAddEventAsync(inputs: [layout, userId]);
        ValidateLayout(layout: layout, parameterName: "entity");


        return eventService.RaiseLayoutAddEventAsync(
            entity: layout,
            userId: userId);

    }, isValueTask: true);

    public ValueTask RaiseLayoutUpdateEventAsync(Layout layout, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseLayoutUpdateEventAsync(inputs: [layout, userId]);
        ValidateLayout(layout: layout, parameterName: "entity");


        return eventService.RaiseLayoutUpdateEventAsync(
            entity: layout,
            userId: userId);

    }, isValueTask: true);

    public ValueTask RaiseLayoutDeleteEventAsync(Layout layout, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseLayoutDeleteEventAsync(inputs: [layout, userId]);
        ValidateLayout(layout: layout, parameterName: "entity");


        return eventService.RaiseLayoutDeleteEventAsync(
            entity: layout,
            userId: userId);

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