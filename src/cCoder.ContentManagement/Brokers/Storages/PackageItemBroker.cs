// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Packaging;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

internal sealed class PackageItemBroker(ICoreContextFactory coreContextFactory) : IPackageItemBroker
{
    public IQueryable<PackageItem> GetAllPackageItems()
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.PackageItems;
    }

    public IQueryable<PackageItem> GetAllPackageItemsIgnoringFilters()
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.PackageItems.IgnoreQueryFilters();
    }

    public async ValueTask<PackageItem> AddPackageItemAsync(PackageItem newPackageItem)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        PackageItem result = (await coreDataContext.PackageItems.AddAsync(entity: newPackageItem)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<PackageItem> UpdatePackageItemAsync(PackageItem updatedPackageItem)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        PackageItem result = coreDataContext.PackageItems.Update(entity: updatedPackageItem)
            .Entity;

        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeletePackageItemAsync(PackageItem deletedPackageItem)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.PackageItems.Remove(entity: deletedPackageItem);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllPackageItemsAsync(IEnumerable<PackageItem> deletedPackageItem)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.PackageItems.RemoveRange(entities: deletedPackageItem);
        await coreDataContext.SaveChangesAsync();
    }

    public int? GetAppId(PackageItem entity) =>
        null;
}