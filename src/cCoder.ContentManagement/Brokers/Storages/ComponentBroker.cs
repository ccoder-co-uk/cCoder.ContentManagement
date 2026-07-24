// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

    public async ValueTask<Component> AddComponentAsync(Component newComponent)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Component result = (await coreDataContext.Components.AddAsync(entity: newComponent)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Component> UpdateComponentAsync(Component updatedComponent)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        Component result = coreDataContext.Components.Update(entity: updatedComponent)
            .Entity;

        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteComponentAsync(Component deletedComponent)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Components.Remove(entity: deletedComponent);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllComponentsAsync(IEnumerable<Component> deletedComponent)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Components.RemoveRange(entities: deletedComponent);
        await coreDataContext.SaveChangesAsync();
    }

}