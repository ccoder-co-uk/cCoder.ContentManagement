// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

internal sealed class AppManager(IAppOrchestrationService appOrchestrationService) : IAppManager
{
    public App Get(int id, bool ignoreFilters = false) =>
        appOrchestrationService.Get(id: id);

    public App GetByDomain(string domain, bool ignoreFilters = false) =>
        appOrchestrationService.GetByDomain(domain: domain, ignoreFilters: ignoreFilters);

    public IQueryable<App> GetAll(bool ignoreFilters = false) =>
        appOrchestrationService.GetAll(ignoreFilters: ignoreFilters);

    public ValueTask<App> AddAsync(App app) =>
        appOrchestrationService.AddAsync(entity: app);

    public ValueTask<App> UpdateAsync(App app) =>
        appOrchestrationService.UpdateAsync(entity: app);

    public ValueTask DeleteAsync(int appId) =>
        appOrchestrationService.DeleteAsync(id: appId);
}