using AppCulture = cCoder.Data.Models.CMS.AppCulture;

namespace cCoder.ContentManagement.Services.Foundations.Events;

public interface IAppCultureEventService
{
    ValueTask RaiseAppCultureAddEventAsync(AppCulture entity);

    ValueTask RaiseAppCultureDeleteEventAsync(AppCulture entity);
}
