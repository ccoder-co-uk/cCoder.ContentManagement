// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class ComponentEventProcessingService(IComponentEventService eventService)
    : IComponentEventProcessingService
{
    public ValueTask RaiseComponentAddEventAsync(Component component, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseComponentAddEventAsync(inputs: [component, userId]);
        ValidateComponent(component: component, parameterName: "entity");


        return eventService.RaiseComponentAddEventAsync(
            entity: component,
            userId: userId);

    }, isValueTask: true);

    public ValueTask RaiseComponentUpdateEventAsync(Component component, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseComponentUpdateEventAsync(inputs: [component, userId]);
        ValidateComponent(component: component, parameterName: "entity");


        return eventService.RaiseComponentUpdateEventAsync(
            entity: component,
            userId: userId);

    }, isValueTask: true);

    public ValueTask RaiseComponentDeleteEventAsync(Component component, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseComponentDeleteEventAsync(inputs: [component, userId]);
        ValidateComponent(component: component, parameterName: "entity");


        return eventService.RaiseComponentDeleteEventAsync(
            entity: component,
            userId: userId);

    }, isValueTask: true);

    private static void ValidateComponent(Component component, string parameterName) =>
        ThrowIf(condition: component == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}