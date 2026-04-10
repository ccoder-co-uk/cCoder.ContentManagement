using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

public interface IAppEventService
{
    ValueTask RaiseAppAddEventAsync(App app);

    ValueTask RaiseAppDeleteEventAsync(App app);

    ValueTask RaiseAppUpdateEventAsync(App app);
}
