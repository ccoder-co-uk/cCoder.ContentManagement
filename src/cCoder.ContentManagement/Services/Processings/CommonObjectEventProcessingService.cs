// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class CommonObjectEventProcessingService(
    ICommonObjectEventService eventService)
        : ICommonObjectEventProcessingService
{
    public ValueTask RaiseCommonObjectAddEventAsync(
        CommonObject commonObject,
        string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseCommonObjectAddEventAsync(inputs: [commonObject, userId]);
        ValidateCommonObject(commonObject: commonObject, parameterName: "entity");


        return eventService.RaiseCommonObjectAddEventAsync(
            entity: commonObject,
            userId: userId);

    }, isValueTask: true);

    public ValueTask RaiseCommonObjectUpdateEventAsync(
        CommonObject commonObject,
        string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseCommonObjectUpdateEventAsync(inputs: [commonObject, userId]);
        ValidateCommonObject(commonObject: commonObject, parameterName: "entity");


        return eventService.RaiseCommonObjectUpdateEventAsync(
            entity: commonObject,
            userId: userId);

    }, isValueTask: true);

    public ValueTask RaiseCommonObjectDeleteEventAsync(
        CommonObject commonObject,
        string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseCommonObjectDeleteEventAsync(inputs: [commonObject, userId]);
        ValidateCommonObject(commonObject: commonObject, parameterName: "entity");


        return eventService.RaiseCommonObjectDeleteEventAsync(
            entity: commonObject,
            userId: userId);

    }, isValueTask: true);

    public ValueTask RaiseCommonObjectsImportedEventAsync(
        CommonObject[] commonObjects,
        string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseCommonObjectsImportedEventAsync(
            inputs: [commonObjects, userId]);


        return eventService.RaiseCommonObjectsImportedEventAsync(
            commonObjects: commonObjects,
            userId: userId);

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