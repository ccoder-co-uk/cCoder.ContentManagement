using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using AppCulture = cCoder.Data.Models.CMS.AppCulture;

namespace cCoder.ContentManagement.Services.Processings;

internal class AppCultureEventProcessingService(IAppCultureEventService eventService) : IAppCultureEventProcessingService
{
    public ValueTask RaiseAppCultureAddEventAsync(AppCulture entity)
    {
        return eventService.RaiseAppCultureAddEventAsync(ValidateAppCulture(entity, "entity"));
    }

    public ValueTask RaiseAppCultureDeleteEventAsync(AppCulture entity)
    {
        return eventService.RaiseAppCultureDeleteEventAsync(ValidateAppCulture(entity, "entity"));
    }

    private static AppCulture ValidateAppCulture(AppCulture appCulture, string parameterName)
    {
        if (appCulture == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return appCulture;
    }
}
