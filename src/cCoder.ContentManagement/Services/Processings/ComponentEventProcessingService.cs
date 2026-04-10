using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using Component = cCoder.Data.Models.CMS.Component;

namespace cCoder.ContentManagement.Services.Processings;

internal class ComponentEventProcessingService(IComponentEventService eventService) : IComponentEventProcessingService
{
    public ValueTask RaiseComponentAddEventAsync(Component entity)
    {
        return eventService.RaiseComponentAddEventAsync(ValidateComponent(entity, "entity"));
    }

    public ValueTask RaiseComponentUpdateEventAsync(Component entity)
    {
        return eventService.RaiseComponentUpdateEventAsync(ValidateComponent(entity, "entity"));
    }

    public ValueTask RaiseComponentDeleteEventAsync(Component entity)
    {
        return eventService.RaiseComponentDeleteEventAsync(ValidateComponent(entity, "entity"));
    }

    private static Component ValidateComponent(Component component, string parameterName)
    {
        if (component == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return component;
    }
}
