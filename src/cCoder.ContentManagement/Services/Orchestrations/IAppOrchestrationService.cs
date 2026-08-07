// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal interface IAppOrchestrationService
{
    App GetApp(int appId);

    ValueTask<App> GetAppForRenderAsync(int appId);

    bool IsAdminApp(int appId, string userName);

    App GetByDomainApp(string domain, bool ignoreFilters = false);

    IQueryable<App> GetAllApp(bool ignoreFilters = false);

    ValueTask<App> AddAppAsync(App newApp);

    ValueTask<App> UpdateAppAsync(App updatedApp);

    ValueTask DeleteAsync(int appId);

    ValueTask HandleAppDeleteAsync(App app);

    ValueTask<IEnumerable<OperationResult<App>>> AddOrUpdateAppResult(IEnumerable<App> newApp);

    ValueTask DeleteAllAppAsync(IEnumerable<App> deletedApp);

    ValueTask UpdatePageOrderAppAsync(int key, App updatedApp);

    App ResolveCurrentApp();
}