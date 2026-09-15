// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class AppCultureEventProcessingService(IAppCultureEventService eventService)
    : IAppCultureEventProcessingService
{
    public ValueTask RaiseAppCultureAddEventAsync(AppCulture appCulture, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseAppCultureAddEventAsync(inputs: [appCulture, userId]);
        ValidateAppCulture(appCulture: appCulture, parameterName: "entity");


        return eventService.RaiseAppCultureAddEventAsync(
            entity: appCulture,
            userId: userId);

    }, isValueTask: true);

    public ValueTask RaiseAppCultureDeleteEventAsync(AppCulture appCulture, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseAppCultureDeleteEventAsync(inputs: [appCulture, userId]);
        ValidateAppCulture(appCulture: appCulture, parameterName: "entity");


        return eventService.RaiseAppCultureDeleteEventAsync(
            entity: appCulture,
            userId: userId);

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