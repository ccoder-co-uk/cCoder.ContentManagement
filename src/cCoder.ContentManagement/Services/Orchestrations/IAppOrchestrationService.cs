// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IAppOrchestrationService
{
    App GetApp(int appId);

    bool IsAdminApp(int appId, string userName);

    App GetByDomainApp(string domain, bool ignoreFilters = false);

    IQueryable<App> GetAllApp(bool ignoreFilters = false);

    ValueTask<App> AddAppAsync(App newApp);

    ValueTask<App> UpdateAppAsync(App updatedApp);

    ValueTask DeleteAsync(int appId);

    ValueTask<IEnumerable<OperationResult<App>>> AddOrUpdateAppResult(IEnumerable<App> newApp);

    ValueTask DeleteAllAppAsync(IEnumerable<App> deletedApp);

    IQueryable<User> GetAppUsers(int appId);

    ValueTask UpdatePageOrderAppAsync(int key, App updatedApp);

    App ResolveCurrentApp();
}