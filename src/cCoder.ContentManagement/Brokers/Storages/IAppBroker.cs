// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IAppBroker
{
    IQueryable<App> GetAllApps(bool ignoreFilters);

    ValueTask<App> AddAppAsync(App newApp);

    ValueTask<App> UpdateAppAsync(App updatedApp);

    ValueTask<int> DeleteAppAsync(App deletedApp);

    ValueTask DeleteAppAggregateAsync(App deletedApp);

    ValueTask DeleteAllAppsAsync(IEnumerable<App> deletedApp);
}