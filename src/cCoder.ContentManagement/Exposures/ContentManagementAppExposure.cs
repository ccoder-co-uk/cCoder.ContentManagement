using cCoder.ContentManagement.Services.Orchestrations;
using App = cCoder.Data.Models.CMS.App;

namespace cCoder.ContentManagement.Exposures;

internal class ContentManagementAppExposure(IAppOrchestrationService appOrchestrationService) : IContentManagementAppExposure
{
    public App Get(int id, bool ignoreFilters = false)
    {
        return appOrchestrationService.Get(id);
    }

    public App GetByDomain(string domain, bool ignoreFilters = false)
    {
        return appOrchestrationService.GetByDomain(domain, ignoreFilters);
    }

    public ValueTask<App> AddAsync(App app)
    {
        return appOrchestrationService.AddAsync(app);
    }

    public ValueTask<App> UpdateAsync(App app)
    {
        return appOrchestrationService.UpdateAsync(app);
    }

    public ValueTask DeleteAsync(int appId)
    {
        return appOrchestrationService.DeleteAsync(appId);
    }
}
