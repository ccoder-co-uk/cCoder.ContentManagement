// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal class ResourceEventProcessingService(IResourceEventService eventService) : IResourceEventProcessingService
{
    public ValueTask RaiseResourceAddEventAsync(Resource entity)
    {
        ValidateResource(resource: entity, parameterName: "entity");

        return eventService.RaiseResourceAddEventAsync(entity: entity);
    }

    public ValueTask RaiseResourceUpdateEventAsync(Resource entity)
    {
        ValidateResource(resource: entity, parameterName: "entity");

        return eventService.RaiseResourceUpdateEventAsync(entity: entity);
    }

    public ValueTask RaiseResourceDeleteEventAsync(Resource entity)
    {
        ValidateResource(resource: entity, parameterName: "entity");

        return eventService.RaiseResourceDeleteEventAsync(entity: entity);
    }

    private static void ValidateResource(Resource resource, string parameterName) =>
        ThrowIf(condition: resource == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}