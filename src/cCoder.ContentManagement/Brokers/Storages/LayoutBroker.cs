using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

public class LayoutBroker(ICoreContextFactory coreContextFactory) : ILayoutBroker
{
    public IQueryable<Layout> GetAllLayouts(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return ignoreFilters
            ? coreDataContext.Layouts.IgnoreQueryFilters()
            : coreDataContext.Layouts;
    }

    public async ValueTask<Layout> AddLayoutAsync(Layout entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Layout result = (await coreDataContext.Layouts.AddAsync(entity)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Layout> UpdateLayoutAsync(Layout entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Layout result = coreDataContext.Layouts.Update(entity).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteLayoutAsync(Layout entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Layouts.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllLayoutsAsync(IEnumerable<Layout> items)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Layouts.RemoveRange(items);
        await coreDataContext.SaveChangesAsync();
    }

}
