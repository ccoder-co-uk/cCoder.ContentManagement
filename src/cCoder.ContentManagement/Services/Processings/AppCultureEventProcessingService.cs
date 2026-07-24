// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class AppCultureEventProcessingService(IAppCultureEventService eventService) : IAppCultureEventProcessingService
{
    public ValueTask RaiseAppCultureAddEventAsync(AppCulture entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseAppCultureAddEventAsync(inputs: [entity]);
        ValidateAppCulture(appCulture: entity, parameterName: "entity");

        return eventService.RaiseAppCultureAddEventAsync(entity: entity);

    }, isValueTask: true);

    public ValueTask RaiseAppCultureDeleteEventAsync(AppCulture entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseAppCultureDeleteEventAsync(inputs: [entity]);
        ValidateAppCulture(appCulture: entity, parameterName: "entity");

        return eventService.RaiseAppCultureDeleteEventAsync(entity: entity);

    }, isValueTask: true);

    private static void ValidateAppCulture(AppCulture appCulture, string parameterName) =>
        ThrowIf(condition: appCulture == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}