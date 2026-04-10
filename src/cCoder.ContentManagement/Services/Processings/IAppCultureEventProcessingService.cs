using AppCulture = cCoder.Data.Models.CMS.AppCulture;

namespace cCoder.ContentManagement.Services.Processings;

public interface IAppCultureEventProcessingService
{
    ValueTask RaiseAppCultureAddEventAsync(AppCulture entity);

    ValueTask RaiseAppCultureDeleteEventAsync(AppCulture entity);
}
