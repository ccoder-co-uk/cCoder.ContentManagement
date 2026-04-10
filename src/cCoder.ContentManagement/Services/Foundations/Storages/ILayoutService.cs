using Layout = cCoder.Data.Models.CMS.Layout;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

public interface ILayoutService
{
    Layout Get(int id, bool ignoreFilters = false);

    IQueryable<Layout> GetAll(bool ignoreFilters = false);

    ValueTask<Layout> AddAsync(Layout layout);

    ValueTask<Layout> UpdateAsync(Layout layout);

    ValueTask DeleteAsync(int id);
}
