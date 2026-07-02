using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

internal sealed class AppManager(IAppOrchestrationService appOrchestrationService) : IAppManager
{
    public App Get(int id, bool ignoreFilters = false) =>
        appOrchestrationService.Get(id);

    public App GetByDomain(string domain, bool ignoreFilters = false) =>
        appOrchestrationService.GetByDomain(domain, ignoreFilters);

    public IQueryable<App> GetAll(bool ignoreFilters = false) =>
        appOrchestrationService.GetAll(ignoreFilters);

    public ValueTask<App> AddAsync(App app) =>
        appOrchestrationService.AddAsync(app);

    public ValueTask<App> UpdateAsync(App app) =>
        appOrchestrationService.UpdateAsync(app);

    public ValueTask DeleteAsync(int appId) =>
        appOrchestrationService.DeleteAsync(appId);
}
