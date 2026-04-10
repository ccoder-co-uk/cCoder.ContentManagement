using cCoder.Data;
using cCoder.Data.Models.Packaging;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

public class PackageItemBroker(ICoreContextFactory coreContextFactory) : IPackageItemBroker
{
    public IQueryable<PackageItem> GetAllPackageItems(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return ignoreFilters
            ? coreDataContext.PackageItems.IgnoreQueryFilters()
            : coreDataContext.PackageItems;
    }

    public async ValueTask<PackageItem> AddPackageItemAsync(PackageItem entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        PackageItem result = (await coreDataContext.PackageItems.AddAsync(entity)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<PackageItem> UpdatePackageItemAsync(PackageItem entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        PackageItem result = coreDataContext.PackageItems.Update(entity).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeletePackageItemAsync(PackageItem entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.PackageItems.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllPackageItemsAsync(IEnumerable<PackageItem> items)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.PackageItems.RemoveRange(items);
        await coreDataContext.SaveChangesAsync();
    }

    public int? GetAppId(PackageItem entity)
    {
        return null;
    }
}
