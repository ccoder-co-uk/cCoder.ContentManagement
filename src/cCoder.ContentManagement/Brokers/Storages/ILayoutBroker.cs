using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface ILayoutBroker
{
    IQueryable<Layout> GetAllLayouts(bool ignoreFilters);

    ValueTask<Layout> AddLayoutAsync(Layout entity);

    ValueTask<Layout> UpdateLayoutAsync(Layout entity);

    ValueTask<int> DeleteLayoutAsync(Layout entity);

    ValueTask DeleteAllLayoutsAsync(IEnumerable<Layout> items);
}
