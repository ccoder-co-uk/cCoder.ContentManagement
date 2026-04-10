using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

public class PageBroker(ICoreContextFactory coreContextFactory) : IPageBroker
{
    public IQueryable<Page> GetAllPages(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return ignoreFilters
            ? coreDataContext.Pages.IgnoreQueryFilters()
            : coreDataContext.Pages;
    }

    public async ValueTask<Page> AddPageAsync(Page entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Page result = (await coreDataContext.Pages.AddAsync(entity)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Page> UpdatePageAsync(Page entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Page result = coreDataContext.Pages.Update(entity).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeletePageAsync(Page entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Pages.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllPagesAsync(IEnumerable<Page> items)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Pages.RemoveRange(items);
        await coreDataContext.SaveChangesAsync();
    }
}
