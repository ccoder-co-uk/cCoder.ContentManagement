using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal class ComponentEventProcessingService(IComponentEventService eventService) : IComponentEventProcessingService
{
    public ValueTask RaiseComponentAddEventAsync(Component entity)
    {
        ValidateComponent(entity, "entity");

        return eventService.RaiseComponentAddEventAsync(entity);
    }

    public ValueTask RaiseComponentUpdateEventAsync(Component entity)
    {
        ValidateComponent(entity, "entity");

        return eventService.RaiseComponentUpdateEventAsync(entity);
    }

    public ValueTask RaiseComponentDeleteEventAsync(Component entity)
    {
        ValidateComponent(entity, "entity");

        return eventService.RaiseComponentDeleteEventAsync(entity);
    }

    private static void ValidateComponent(Component component, string parameterName) =>
        ThrowIf(component == null, parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
