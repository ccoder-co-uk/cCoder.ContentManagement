// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class ResourceEventProcessingService(IResourceEventService eventService)
    : IResourceEventProcessingService
{
    public ValueTask RaiseResourceAddEventAsync(Resource resource, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseResourceAddEventAsync(inputs: [resource, userId]);
        ValidateResource(resource: resource, parameterName: "entity");


        return eventService.RaiseResourceAddEventAsync(
            entity: resource,
            userId: userId);

    }, isValueTask: true);

    public ValueTask RaiseResourceUpdateEventAsync(Resource resource, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseResourceUpdateEventAsync(inputs: [resource, userId]);
        ValidateResource(resource: resource, parameterName: "entity");


        return eventService.RaiseResourceUpdateEventAsync(
            entity: resource,
            userId: userId);

    }, isValueTask: true);

    public ValueTask RaiseResourceDeleteEventAsync(Resource resource, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseResourceDeleteEventAsync(inputs: [resource, userId]);
        ValidateResource(resource: resource, parameterName: "entity");


        return eventService.RaiseResourceDeleteEventAsync(
            entity: resource,
            userId: userId);

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