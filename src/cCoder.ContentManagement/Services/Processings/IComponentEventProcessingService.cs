using Component = cCoder.Data.Models.CMS.Component;

namespace cCoder.ContentManagement.Services.Processings;

public interface IComponentEventProcessingService
{
    ValueTask RaiseComponentAddEventAsync(Component entity);

    ValueTask RaiseComponentUpdateEventAsync(Component entity);

    ValueTask RaiseComponentDeleteEventAsync(Component entity);
}
