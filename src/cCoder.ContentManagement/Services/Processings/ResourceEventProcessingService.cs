using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal class ResourceEventProcessingService(IResourceEventService eventService) : IResourceEventProcessingService
{
    public ValueTask RaiseResourceAddEventAsync(Resource entity)
    {
        ValidateResource(entity, "entity");

        return eventService.RaiseResourceAddEventAsync(entity);
    }

    public ValueTask RaiseResourceUpdateEventAsync(Resource entity)
    {
        ValidateResource(entity, "entity");

        return eventService.RaiseResourceUpdateEventAsync(entity);
    }

    public ValueTask RaiseResourceDeleteEventAsync(Resource entity)
    {
        ValidateResource(entity, "entity");

        return eventService.RaiseResourceDeleteEventAsync(entity);
    }

    private static void ValidateResource(Resource resource, string parameterName) =>
        ThrowIf(resource == null, parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
