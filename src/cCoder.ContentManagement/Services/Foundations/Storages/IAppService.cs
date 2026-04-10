using App = cCoder.Data.Models.CMS.App;
using Page = cCoder.Data.Models.CMS.Page;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

public interface IAppService
{
    App Get(int id, bool ignoreFilters = false);

    IQueryable<App> GetAll(bool ignoreFilters = false);

    ValueTask<App> AddAsync(App app);

    ValueTask<App> UpdateAsync(App app);

    ValueTask UpdatePageOrderAsync(int id, IEnumerable<Page> pages);

    ValueTask DeleteAsync(int id);
}
