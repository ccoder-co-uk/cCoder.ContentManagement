using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IAppBroker
{
    IQueryable<App> GetAllApps(bool ignoreFilters);

    ValueTask<App> AddAppAsync(App entity);

    ValueTask<App> UpdateAppAsync(App entity);

    ValueTask<int> DeleteAppAsync(App entity);

    ValueTask DeleteAppAggregateAsync(App entity);

    ValueTask DeleteAllAppsAsync(IEnumerable<App> items);
}
