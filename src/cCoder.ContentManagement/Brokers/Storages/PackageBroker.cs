// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Packaging;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

internal sealed class PackageBroker(ICoreContextFactory coreContextFactory) : IPackageBroker
{
    public IQueryable<Package> GetAllPackages(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return Dependencies.QueryFilterDependency.Apply(
            query: coreDataContext.Packages,
            ignoreFilters: ignoreFilters);
    }

    public async ValueTask<Package> AddPackageAsync(Package newPackage)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Package result = (await coreDataContext.Packages.AddAsync(entity: newPackage)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Package> UpdatePackageAsync(Package updatedPackage)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        Package result = coreDataContext.Packages.Update(entity: updatedPackage)
            .Entity;

        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeletePackageAsync(Package deletedPackage)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Packages.Remove(entity: deletedPackage);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllPackagesAsync(IEnumerable<Package> deletedPackage)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Packages.RemoveRange(entities: deletedPackage);
        await coreDataContext.SaveChangesAsync();
    }
}