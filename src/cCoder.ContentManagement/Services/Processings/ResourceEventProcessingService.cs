using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using Resource = cCoder.Data.Models.CMS.Resource;

namespace cCoder.ContentManagement.Services.Processings;

internal class ResourceEventProcessingService(IResourceEventService eventService) : IResourceEventProcessingService
{
    public ValueTask RaiseResourceAddEventAsync(Resource entity)
    {
        return eventService.RaiseResourceAddEventAsync(ValidateResource(entity, "entity"));
    }

    public ValueTask RaiseResourceUpdateEventAsync(Resource entity)
    {
        return eventService.RaiseResourceUpdateEventAsync(ValidateResource(entity, "entity"));
    }

    public ValueTask RaiseResourceDeleteEventAsync(Resource entity)
    {
        return eventService.RaiseResourceDeleteEventAsync(ValidateResource(entity, "entity"));
    }

    private static Resource ValidateResource(Resource resource, string parameterName)
    {
        if (resource == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return resource;
    }
}
