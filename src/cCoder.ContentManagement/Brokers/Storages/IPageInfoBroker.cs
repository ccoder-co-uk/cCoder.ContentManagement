using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IPageInfoBroker
{
    IQueryable<PageInfo> GetAllPageInfo(bool ignoreFilters);

    ValueTask<PageInfo> AddPageInfoAsync(PageInfo entity);

    ValueTask<PageInfo> UpdatePageInfoAsync(PageInfo entity);

    ValueTask<int> DeletePageInfoAsync(PageInfo entity);

    ValueTask DeleteAllPageInfoAsync(IEnumerable<PageInfo> items);
}
