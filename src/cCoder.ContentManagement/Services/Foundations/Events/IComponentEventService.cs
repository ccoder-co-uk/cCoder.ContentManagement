using Component = cCoder.Data.Models.CMS.Component;

namespace cCoder.ContentManagement.Services.Foundations.Events;

public interface IComponentEventService
{
    ValueTask RaiseComponentAddEventAsync(Component entity);

    ValueTask RaiseComponentUpdateEventAsync(Component entity);

    ValueTask RaiseComponentDeleteEventAsync(Component entity);
}
