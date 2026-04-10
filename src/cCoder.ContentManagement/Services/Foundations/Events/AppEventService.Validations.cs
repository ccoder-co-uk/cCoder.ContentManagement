using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class AppEventService
{
    private static App ValidateApp(App app, string parameterName)
    {
        if (app == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }

        return app;
    }
}
