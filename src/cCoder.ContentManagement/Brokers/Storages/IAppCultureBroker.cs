using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IAppCultureBroker
{
    IQueryable<AppCulture> GetAllAppCultures(bool ignoreFilters);

    ValueTask<AppCulture> AddAppCultureAsync(AppCulture entity);

    ValueTask<int> DeleteAppCultureAsync(AppCulture entity);

    ValueTask DeleteAllAppCulturesAsync(IEnumerable<AppCulture> items);
}
