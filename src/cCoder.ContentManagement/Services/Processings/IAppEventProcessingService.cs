using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

public interface IAppEventProcessingService
{
    ValueTask RaiseAppAddEventAsync(App app);

    ValueTask RaiseAppDeleteEventAsync(App app);

    ValueTask RaiseAppUpdateEventAsync(App app);
}
