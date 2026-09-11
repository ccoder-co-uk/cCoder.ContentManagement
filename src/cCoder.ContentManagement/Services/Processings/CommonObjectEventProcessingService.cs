// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class CommonObjectEventProcessingService(ICommonObjectEventService eventService) : ICommonObjectEventProcessingService
{
    public ValueTask RaiseCommonObjectAddEventAsync(CommonObject commonObject) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseCommonObjectAddEventAsync(inputs: [commonObject]);
        ValidateCommonObject(commonObject: commonObject, parameterName: "entity");

        return eventService.RaiseCommonObjectAddEventAsync(entity: commonObject);

    }, isValueTask: true);

    public ValueTask RaiseCommonObjectUpdateEventAsync(CommonObject commonObject) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseCommonObjectUpdateEventAsync(inputs: [commonObject]);
        ValidateCommonObject(commonObject: commonObject, parameterName: "entity");

        return eventService.RaiseCommonObjectUpdateEventAsync(entity: commonObject);

    }, isValueTask: true);

    public ValueTask RaiseCommonObjectDeleteEventAsync(CommonObject commonObject) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseCommonObjectDeleteEventAsync(inputs: [commonObject]);
        ValidateCommonObject(commonObject: commonObject, parameterName: "entity");

        return eventService.RaiseCommonObjectDeleteEventAsync(entity: commonObject);

    }, isValueTask: true);

    public ValueTask RaiseCommonObjectsImportedEventAsync(
        CommonObject[] commonObjects) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseCommonObjectsImportedEventAsync(
            inputs: [commonObjects]);

        return eventService.RaiseCommonObjectsImportedEventAsync(
            commonObjects: commonObjects);

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