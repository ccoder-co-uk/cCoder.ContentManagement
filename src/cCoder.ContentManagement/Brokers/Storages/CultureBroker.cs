using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

public class CultureBroker(ICoreContextFactory coreContextFactory) : ICultureBroker
{
    public IQueryable<Culture> GetAllCultures(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return ignoreFilters
            ? coreDataContext.Cultures.IgnoreQueryFilters()
            : coreDataContext.Cultures;
    }

    public async ValueTask<Culture> AddCultureAsync(Culture entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Culture result = (await coreDataContext.Cultures.AddAsync(entity)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Culture> UpdateCultureAsync(Culture entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Culture result = coreDataContext.Cultures.Update(entity).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteCultureAsync(Culture entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Cultures.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllCulturesAsync(IEnumerable<Culture> items)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Cultures.RemoveRange(items);
        await coreDataContext.SaveChangesAsync();
    }
}
