// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

internal sealed class AppManager(IAppOrchestrationService appOrchestrationService) : IAppManager
{
    public App Get(int appManagerId, bool ignoreFilters = false) =>
        appOrchestrationService.GetApp(appId: appManagerId);

    public App GetByDomain(string domain, bool ignoreFilters = false) =>
        appOrchestrationService.GetByDomainApp(domain: domain, ignoreFilters: ignoreFilters);

    public IQueryable<App> GetAll(bool ignoreFilters = false) =>
        appOrchestrationService.GetAllApp(ignoreFilters: ignoreFilters);

    public ValueTask<App> AddAsync(App newApp) =>
        appOrchestrationService.AddAppAsync(newApp: newApp);

    public ValueTask<App> UpdateAsync(App updatedApp) =>
        appOrchestrationService.UpdateAppAsync(updatedApp: updatedApp);

    public ValueTask DeleteAsync(int appId) =>
        appOrchestrationService.DeleteAsync(appId: appId);
}