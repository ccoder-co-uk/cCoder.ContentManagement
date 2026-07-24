// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal class CommonObjectEventProcessingService(ICommonObjectEventService eventService) : ICommonObjectEventProcessingService
{
    public ValueTask RaiseCommonObjectAddEventAsync(CommonObject entity)
    {
        ValidateCommonObject(commonObject: entity, parameterName: "entity");

        return eventService.RaiseCommonObjectAddEventAsync(entity: entity);
    }

    public ValueTask RaiseCommonObjectUpdateEventAsync(CommonObject entity)
    {
        ValidateCommonObject(commonObject: entity, parameterName: "entity");

        return eventService.RaiseCommonObjectUpdateEventAsync(entity: entity);
    }

    public ValueTask RaiseCommonObjectDeleteEventAsync(CommonObject entity)
    {
        ValidateCommonObject(commonObject: entity, parameterName: "entity");

        return eventService.RaiseCommonObjectDeleteEventAsync(entity: entity);
    }

    private static void ValidateCommonObject(CommonObject commonObject, string parameterName) =>
        ThrowIf(condition: commonObject == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}