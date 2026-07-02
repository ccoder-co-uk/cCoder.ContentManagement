using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal class AppCultureEventProcessingService(IAppCultureEventService eventService) : IAppCultureEventProcessingService
{
    public ValueTask RaiseAppCultureAddEventAsync(AppCulture entity)
    {
        ValidateAppCulture(entity, "entity");

        return eventService.RaiseAppCultureAddEventAsync(entity);
    }

    public ValueTask RaiseAppCultureDeleteEventAsync(AppCulture entity)
    {
        ValidateAppCulture(entity, "entity");

        return eventService.RaiseAppCultureDeleteEventAsync(entity);
    }

    private static void ValidateAppCulture(AppCulture appCulture, string parameterName) =>
        ThrowIf(appCulture == null, parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
