// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class ComponentEventProcessingService(IComponentEventService eventService) : IComponentEventProcessingService
{
    public ValueTask RaiseComponentAddEventAsync(Component entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseComponentAddEventAsync(inputs: [entity]);
        ValidateComponent(component: entity, parameterName: "entity");

        return eventService.RaiseComponentAddEventAsync(entity: entity);

    }, isValueTask: true);

    public ValueTask RaiseComponentUpdateEventAsync(Component entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseComponentUpdateEventAsync(inputs: [entity]);
        ValidateComponent(component: entity, parameterName: "entity");

        return eventService.RaiseComponentUpdateEventAsync(entity: entity);

    }, isValueTask: true);

    public ValueTask RaiseComponentDeleteEventAsync(Component entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseComponentDeleteEventAsync(inputs: [entity]);
        ValidateComponent(component: entity, parameterName: "entity");

        return eventService.RaiseComponentDeleteEventAsync(entity: entity);

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