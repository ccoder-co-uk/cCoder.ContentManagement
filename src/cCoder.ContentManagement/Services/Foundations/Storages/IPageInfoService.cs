using PageInfo = cCoder.Data.Models.CMS.PageInfo;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

public interface IPageInfoService
{
    PageInfo Get(int id, bool ignoreFilters = false);

    IQueryable<PageInfo> GetAll(bool ignoreFilters = false);

    ValueTask<PageInfo> AddAsync(PageInfo pageInfo);

    ValueTask<PageInfo> UpdateAsync(PageInfo pageInfo);

    ValueTask DeleteAsync(int id);
}
