using cCoder.Data;
using cCoder.Data.Models.Packaging;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

public class PackageBroker(ICoreContextFactory coreContextFactory) : IPackageBroker
{
    public IQueryable<Package> GetAllPackages(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return ignoreFilters
            ? coreDataContext.Packages.IgnoreQueryFilters()
            : coreDataContext.Packages;
    }

    public async ValueTask<Package> AddPackageAsync(Package entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Package result = (await coreDataContext.Packages.AddAsync(entity)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Package> UpdatePackageAsync(Package entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Package result = coreDataContext.Packages.Update(entity).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeletePackageAsync(Package entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Packages.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllPackagesAsync(IEnumerable<Package> items)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Packages.RemoveRange(items);
        await coreDataContext.SaveChangesAsync();
    }
}
