// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IAppOrchestrationService
{
    App GetApp(int appId);

    ValueTask<App> GetAppForRenderAsync(int appId);

    bool IsAdminApp(int appId, string userName);

    App GetByDomainApp(string domain, bool ignoreFilters = false);

    IQueryable<App> GetAllApp(bool ignoreFilters = false);

    ValueTask<App> AddAppAsync(App newApp);

    ValueTask<App> UpdateAppAsync(App updatedApp);

    ValueTask DeleteAppAsync(int appId);

    ValueTask HandleAppDeleteAsync(App app);

    ValueTask DeleteAllAppAsync(IEnumerable<App> deletedApp);
}