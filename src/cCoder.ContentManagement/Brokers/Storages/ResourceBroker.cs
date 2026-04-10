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

    public async ValueTask<Resource> AddResourceAsync(Resource entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Resource result = (await coreDataContext.Resources.AddAsync(entity)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Resource> UpdateResourceAsync(Resource entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Resource result = coreDataContext.Resources.Update(entity).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteResourceAsync(Resource entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Resources.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllResourcesAsync(IEnumerable<Resource> items)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Resources.RemoveRange(items);
        await coreDataContext.SaveChangesAsync();
    }

}
