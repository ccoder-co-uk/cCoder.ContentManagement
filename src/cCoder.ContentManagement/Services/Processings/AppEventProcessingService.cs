using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Services.Foundations.Events;

namespace cCoder.ContentManagement.Services.Processings;

internal class AppEventProcessingService(IAppEventService eventService) : IAppEventProcessingService
{
    public ValueTask RaiseAppAddEventAsync(App app)
    {
        return eventService.RaiseAppAddEventAsync(ValidateApp(app, "app"));
    }

    public ValueTask RaiseAppDeleteEventAsync(App app)
    {
        return eventService.RaiseAppDeleteEventAsync(ValidateApp(app, "app"));
    }

    public ValueTask RaiseAppUpdateEventAsync(App app)
    {
        return eventService.RaiseAppUpdateEventAsync(ValidateApp(app, "app"));
    }

    private static App ValidateApp(App app, string parameterName)
    {
        if (app == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return app;
    }
}
