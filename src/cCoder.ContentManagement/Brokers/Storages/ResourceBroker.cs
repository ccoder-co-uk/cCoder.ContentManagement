// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

public class ResourceBroker(ICoreContextFactory coreContextFactory) : IResourceBroker
{
    public IQueryable<Resource> GetAllResources(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return ignoreFilters
            ? coreDataContext.Resources.IgnoreQueryFilters()
            : coreDataContext.Resources;
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