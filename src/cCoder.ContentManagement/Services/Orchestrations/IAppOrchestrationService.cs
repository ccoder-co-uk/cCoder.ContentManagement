using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IAppOrchestrationService
{
    App Get(int id);

    App GetByDomain(string domain, bool ignoreFilters = false);

    IQueryable<App> GetAll(bool ignoreFilters = false);

    ValueTask<App> AddAsync(App entity);

    ValueTask<App> UpdateAsync(App entity);

    ValueTask DeleteAsync(int id);

    ValueTask<IEnumerable<Result<App>>> AddOrUpdate(IEnumerable<App> items);

    ValueTask DeleteAllAsync(IEnumerable<App> items);

    IQueryable<User> GetAppUsers(int appId);

    ValueTask UpdatePageOrderAsync(int key, App app);

    App ResolveCurrentApp();
}
