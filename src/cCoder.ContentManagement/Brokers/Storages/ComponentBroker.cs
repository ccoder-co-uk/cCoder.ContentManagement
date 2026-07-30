// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

internal sealed class ComponentBroker(ICoreContextFactory coreContextFactory) : IComponentBroker
{
    public IQueryable<Component> GetAllComponents()
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.Components;
    }

    public IQueryable<Component> GetAllComponentsIgnoringFilters()
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.Components.IgnoreQueryFilters();
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