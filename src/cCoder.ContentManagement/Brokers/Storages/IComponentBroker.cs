using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IComponentBroker
{
    IQueryable<Component> GetAllComponents(bool ignoreFilters);

    ValueTask<Component> AddComponentAsync(Component entity);

    ValueTask<Component> UpdateComponentAsync(Component entity);

    ValueTask<int> DeleteComponentAsync(Component entity);

    ValueTask DeleteAllComponentsAsync(IEnumerable<Component> items);
}
