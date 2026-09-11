// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class ResourceEventProcessingService(IResourceEventService eventService) : IResourceEventProcessingService
{
    public ValueTask RaiseResourceAddEventAsync(Resource resource) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseResourceAddEventAsync(inputs: [resource]);
        ValidateResource(resource: resource, parameterName: "entity");

        return eventService.RaiseResourceAddEventAsync(entity: resource);

    }, isValueTask: true);

    public ValueTask RaiseResourceUpdateEventAsync(Resource resource) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseResourceUpdateEventAsync(inputs: [resource]);
        ValidateResource(resource: resource, parameterName: "entity");

        return eventService.RaiseResourceUpdateEventAsync(entity: resource);

    }, isValueTask: true);

    public ValueTask RaiseResourceDeleteEventAsync(Resource resource) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseResourceDeleteEventAsync(inputs: [resource]);
        ValidateResource(resource: resource, parameterName: "entity");

        return eventService.RaiseResourceDeleteEventAsync(entity: resource);

    }, isValueTask: true);

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