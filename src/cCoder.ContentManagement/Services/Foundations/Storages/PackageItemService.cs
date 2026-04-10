using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using PackageItem = cCoder.Data.Models.Packaging.PackageItem;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PackageItemService(IPackageItemBroker packageItemBroker, IAuthorizationBroker authorizationBroker) : IPackageItemService
{
    public PackageItem Get(Guid id, bool ignoreFilters = false)
    {
        if (ignoreFilters)
        {
            return GetAll(ignoreFilters: true).FirstOrDefault((PackageItem i) => i.Id == id);
        }

        PackageItem packageItem = GetAll().FirstOrDefault((PackageItem i) => i.Id == id);
        if (packageItem != null)
        {
            return packageItem;
        }
        PackageItem packageItem2 = GetAll(ignoreFilters: true).FirstOrDefault((PackageItem i) => i.Id == id);
        if (packageItem2 != null)
        {
            throw new SecurityException("Access Denied!");
        }
        return null;
    }

    public IQueryable<PackageItem> GetAll(bool ignoreFilters = false)
    {
        return packageItemBroker.GetAllPackageItems(ignoreFilters);
    }

    public async ValueTask<PackageItem> AddAsync(PackageItem packageItem)
    {
        PackageItem dataPackageItem = CreateStoragePackageItem(packageItem);
        authorizationBroker.Authorize(packageItemBroker.GetAppId(dataPackageItem), "PackageItem_create");
        PackageItem result = await packageItemBroker.AddPackageItemAsync(dataPackageItem);
        result.Package = packageItem.Package;
        return result;
    }

    public async ValueTask<PackageItem> UpdateAsync(PackageItem packageItem)
    {
        PackageItem dataPackageItem = CreateStoragePackageItem(packageItem);
        authorizationBroker.Authorize(packageItemBroker.GetAppId(dataPackageItem), "PackageItem_update");
        PackageItem result = await packageItemBroker.UpdatePackageItemAsync(dataPackageItem);
        result.Package = packageItem.Package;
        return result;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        PackageItem packageItem = Get(id);
        PackageItem dataPackageItem = CreateStoragePackageItem(packageItem);
        authorizationBroker.Authorize(packageItemBroker.GetAppId(dataPackageItem), "PackageItem_delete");
        await packageItemBroker.DeletePackageItemAsync(dataPackageItem);
    }

    private static PackageItem CreateStoragePackageItem(PackageItem packageItem)
    {
        if (packageItem == null)
        {
            return null;
        }

        return new PackageItem
        {
            Id = packageItem.Id,
            PackageId = packageItem.PackageId,
            Type = packageItem.Type,
            Data = packageItem.Data
        };
    }
}
