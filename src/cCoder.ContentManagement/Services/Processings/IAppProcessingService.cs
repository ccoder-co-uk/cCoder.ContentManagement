using CommonObject = cCoder.Data.Models.CommonObject;
using App = cCoder.Data.Models.CMS.App;
using Page = cCoder.Data.Models.CMS.Page;
using User = cCoder.Data.Models.Security.User;

namespace cCoder.ContentManagement.Services.Processings;

public interface IAppProcessingService
{
    App Get(int id);

    string GetDomain(int id, bool ignoreFilters = false);

    App GetByDomain(string domain, bool ignoreFilters = false);

    IQueryable<App> GetAll(bool ignoreFilters = false);

    ValueTask<App> AddAsync(App entity);

    ValueTask<App> UpdateAsync(App entity);

    ValueTask DeleteAsync(int id);

    ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<App>>> AddOrUpdate(IEnumerable<App> items);

    ValueTask DeleteAllAsync(IEnumerable<App> items);

    IQueryable<User> GetAppUsers(int appId);

    ValueTask UpdatePageOrderAsync(int key, App app);

    App ResolveCurrentApp();
}
