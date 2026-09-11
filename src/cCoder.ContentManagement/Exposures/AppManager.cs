// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Exposures;

internal sealed class AppManager(
    IAppManagerAggregationService appManagerAggregationService)
        : IAppManager
{
    public App Get(int appManagerId, bool ignoreFilters = false) =>
        appManagerAggregationService.GetAppManagerContext(
            appManagerContext: new AppManagerContext { AppId = appManagerId })
        .App;

    public App GetByDomain(string domain, bool ignoreFilters = false) =>
        appManagerAggregationService.GetByDomainAppManagerContext(
            appManagerContext: new AppManagerContext
            {
                Domain = domain,
                IgnoreFilters = ignoreFilters
            })
        .App;

    public IQueryable<App> GetAll(bool ignoreFilters = false) =>
        appManagerAggregationService.GetAllAppManagerContext(
            appManagerContext: new AppManagerContext { IgnoreFilters = ignoreFilters })
        .Apps;

    public async ValueTask<App> AddAsync(App newApp) =>
        (await appManagerAggregationService.AddAppManagerContextAsync(
            newAppManagerContext: new AppManagerContext { App = newApp }))
        .App;

    public async ValueTask<App> UpdateAsync(App updatedApp) =>
        (await appManagerAggregationService.UpdateAppManagerContextAsync(
            updatedAppManagerContext: new AppManagerContext { App = updatedApp }))
        .App;

    public ValueTask DeleteAsync(int appId) =>
        appManagerAggregationService.DeleteAppManagerContextAsync(
            deletedAppManagerContext: new AppManagerContext { AppId = appId });

    public bool IsAdmin(int appId, string userName) =>
        appManagerAggregationService.GetAdminAppManagerContext(
            appManagerContext: new AppManagerContext
            {
                AppId = appId,
                UserName = userName
            })
        .IsAdmin;

    public IQueryable<User> GetUsers(int appId) =>
        appManagerAggregationService.GetUsersAppManagerContext(
            appManagerContext: new AppManagerContext { AppId = appId })
        .Users;

    public ValueTask UpdatePageOrderAsync(int appId, App updatedApp) =>
        appManagerAggregationService.UpdatePageOrderAppManagerContextAsync(
            updatedAppManagerContext: new AppManagerContext
            {
                AppId = appId,
                App = updatedApp
            });
}