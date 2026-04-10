using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

public class ComponentBroker(ICoreContextFactory coreContextFactory) : IComponentBroker
{
    public IQueryable<Component> GetAllComponents(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return ignoreFilters
            ? coreDataContext.Components.IgnoreQueryFilters()
            : coreDataContext.Components;
    }

    public async ValueTask<Component> AddComponentAsync(Component entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Component result = (await coreDataContext.Components.AddAsync(entity)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Component> UpdateComponentAsync(Component entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Component result = coreDataContext.Components.Update(entity).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteComponentAsync(Component entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Components.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllComponentsAsync(IEnumerable<Component> items)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Components.RemoveRange(items);
        await coreDataContext.SaveChangesAsync();
    }

}
