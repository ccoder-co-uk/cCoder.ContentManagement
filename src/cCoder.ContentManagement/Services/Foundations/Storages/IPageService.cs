using Page = cCoder.Data.Models.CMS.Page;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

public interface IPageService
{
    Page Get(int id, bool ignoreFilters = false);

    IQueryable<Page> GetAll(bool ignoreFilters = false);

    ValueTask<Page> AddAsync(Page page);

    ValueTask<Page> UpdateAsync(Page page);

    ValueTask DeleteAsync(int id);
}
