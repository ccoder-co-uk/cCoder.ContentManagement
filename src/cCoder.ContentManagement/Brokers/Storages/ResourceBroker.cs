// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

internal sealed class ResourceBroker(ICoreContextFactory coreContextFactory) : IResourceBroker
{
    public IQueryable<Resource> GetAllResources()
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.Resources;
    }

    public IQueryable<Resource> GetAllResourcesIgnoringFilters()
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.Resources.IgnoreQueryFilters();
    }

    public async ValueTask<Resource> AddResourceAsync(Resource newResource)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Resource result = (await coreDataContext.Resources.AddAsync(entity: newResource)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Resource> UpdateResourceAsync(Resource updatedResource)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        Resource result = coreDataContext.Resources.Update(entity: updatedResource)
            .Entity;

        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteResourceAsync(Resource deletedResource)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Resources.Remove(entity: deletedResource);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllResourcesAsync(IEnumerable<Resource> deletedResource)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Resources.RemoveRange(entities: deletedResource);
        await coreDataContext.SaveChangesAsync();
    }

}