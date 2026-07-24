// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class CommonObjectEventProcessingService(ICommonObjectEventService eventService) : ICommonObjectEventProcessingService
{
    public ValueTask RaiseCommonObjectAddEventAsync(CommonObject entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseCommonObjectAddEventAsync(inputs: [entity]);
        ValidateCommonObject(commonObject: entity, parameterName: "entity");

        return eventService.RaiseCommonObjectAddEventAsync(entity: entity);

    }, isValueTask: true);

    public ValueTask RaiseCommonObjectUpdateEventAsync(CommonObject entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseCommonObjectUpdateEventAsync(inputs: [entity]);
        ValidateCommonObject(commonObject: entity, parameterName: "entity");

        return eventService.RaiseCommonObjectUpdateEventAsync(entity: entity);

    }, isValueTask: true);

    public ValueTask RaiseCommonObjectDeleteEventAsync(CommonObject entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseCommonObjectDeleteEventAsync(inputs: [entity]);
        ValidateCommonObject(commonObject: entity, parameterName: "entity");

        return eventService.RaiseCommonObjectDeleteEventAsync(entity: entity);

    }, isValueTask: true);

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