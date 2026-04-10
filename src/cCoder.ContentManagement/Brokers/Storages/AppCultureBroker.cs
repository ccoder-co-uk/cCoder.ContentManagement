using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

public class AppCultureBroker(ICoreContextFactory coreContextFactory) : IAppCultureBroker
{
    public IQueryable<AppCulture> GetAllAppCultures(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return ignoreFilters
            ? coreDataContext.AppCultures.IgnoreQueryFilters()
            : coreDataContext.AppCultures;
    }

    public async ValueTask<AppCulture> AddAppCultureAsync(AppCulture entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        AppCulture result = (await coreDataContext.AppCultures.AddAsync(entity)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteAppCultureAsync(AppCulture entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.AppCultures.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllAppCulturesAsync(IEnumerable<AppCulture> items)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.AppCultures.RemoveRange(items);
        await coreDataContext.SaveChangesAsync();
    }
}
