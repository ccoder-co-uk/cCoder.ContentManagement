using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IPageBroker
{
    IQueryable<Page> GetAllPages(bool ignoreFilters);

    ValueTask<Page> AddPageAsync(Page entity);

    ValueTask<Page> UpdatePageAsync(Page entity);

    ValueTask<int> DeletePageAsync(Page entity);

    ValueTask DeleteAllPagesAsync(IEnumerable<Page> items);
}
