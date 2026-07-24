// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Services.Foundations.Events;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class AppEventProcessingService(IAppEventService eventService) : IAppEventProcessingService
{
    public ValueTask RaiseAppAddEventAsync(App app) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseAppAddEventAsync(inputs: [app]);
        ValidateApp(app: app, parameterName: "app");

        return eventService.RaiseAppAddEventAsync(app: app);

    }, isValueTask: true);

    public ValueTask RaiseAppDeleteEventAsync(App app) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseAppDeleteEventAsync(inputs: [app]);
        ValidateApp(app: app, parameterName: "app");

        return eventService.RaiseAppDeleteEventAsync(app: app);

    }, isValueTask: true);

    public ValueTask RaiseAppUpdateEventAsync(App app) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseAppUpdateEventAsync(inputs: [app]);
        ValidateApp(app: app, parameterName: "app");

        return eventService.RaiseAppUpdateEventAsync(app: app);

    }, isValueTask: true);

    private static void ValidateApp(App app, string parameterName) =>
        ThrowIf(condition: app == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}