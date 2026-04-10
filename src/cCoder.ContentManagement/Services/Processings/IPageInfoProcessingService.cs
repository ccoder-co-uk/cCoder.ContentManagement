using PageInfo = cCoder.Data.Models.CMS.PageInfo;

namespace cCoder.ContentManagement.Services.Processings;

public interface IPageInfoProcessingService
{
    PageInfo Get(int id);

    IQueryable<PageInfo> GetAll(bool ignoreFilters = false);

    ValueTask<PageInfo> AddAsync(PageInfo entity);

    ValueTask<PageInfo> UpdateAsync(PageInfo entity);

    ValueTask DeleteAsync(int id);

    ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<PageInfo>>> AddOrUpdate(IEnumerable<PageInfo> items);

    ValueTask DeleteAllAsync(IEnumerable<PageInfo> items);
}
