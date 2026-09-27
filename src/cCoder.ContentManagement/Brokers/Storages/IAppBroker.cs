// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IAppBroker
{
    IQueryable<App> GetAllApps();

    IQueryable<App> GetAllAppsIgnoringFilters();

    ValueTask<App> GetAppForRenderAsync(int appId);

    App GetAppForDelete(int appId);

    Culture[] GetCultures();

    Privilege[] GetPrivileges();

    User GetCurrentUser();

    string GetCurrentUserId();

    ValueTask<App> AddAppAsync(App newApp);

    ValueTask<App> UpdateAppAsync(App updatedApp);

    ValueTask PersistNewAppRolesAsync(App app);

    ValueTask<int> DeleteAppAsync(App deletedApp);

    ValueTask DeleteAppAggregateAsync(App deletedApp);

    ValueTask DeleteAllAppsAsync(IEnumerable<App> deletedApp);
}