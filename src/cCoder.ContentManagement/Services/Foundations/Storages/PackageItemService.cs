// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PackageItemService(IPackageItemBroker packageItemBroker, IAuthorizationBroker authorizationBroker) : IPackageItemService
{
    public PackageItem Get(Guid id, bool ignoreFilters = false)
    {
        if (ignoreFilters)
        {
            return GetAll(ignoreFilters: true)
                        .FirstOrDefault(predicate: (PackageItem i) => i.Id == id);
        }

        PackageItem packageItem = GetAll()
            .FirstOrDefault(predicate: (PackageItem i) => i.Id == id);

        if (packageItem != null)
        {
            return packageItem;
        }

        PackageItem packageItem2 = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: (PackageItem i) => i.Id == id);

        if (packageItem2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }

    public IQueryable<PackageItem> GetAll(bool ignoreFilters = false) =>
        packageItemBroker.GetAllPackageItems(ignoreFilters: ignoreFilters);

    public async ValueTask<PackageItem> AddAsync(PackageItem packageItem)
    {
        PackageItem dataPackageItem = CreateStoragePackageItem(packageItem: packageItem);
        authorizationBroker.Authorize(appId: packageItemBroker.GetAppId(entity: dataPackageItem), privilege: "PackageItem_create");
        PackageItem result = await packageItemBroker.AddPackageItemAsync(entity: dataPackageItem);
        packageItem.Id = result.Id;
        packageItem.PackageId = result.PackageId;
        packageItem.Type = result.Type;
        packageItem.Data = result.Data;
        return packageItem;
    }

    public async ValueTask<PackageItem> UpdateAsync(PackageItem packageItem)
    {
        PackageItem dataPackageItem = CreateStoragePackageItem(packageItem: packageItem);
        authorizationBroker.Authorize(appId: packageItemBroker.GetAppId(entity: dataPackageItem), privilege: "PackageItem_update");
        PackageItem result = await packageItemBroker.UpdatePackageItemAsync(entity: dataPackageItem);
        packageItem.Id = result.Id;
        packageItem.PackageId = result.PackageId;
        packageItem.Type = result.Type;
        packageItem.Data = result.Data;
        return packageItem;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        PackageItem packageItem = Get(id: id);
        PackageItem dataPackageItem = CreateStoragePackageItem(packageItem: packageItem);
        authorizationBroker.Authorize(appId: packageItemBroker.GetAppId(entity: dataPackageItem), privilege: "PackageItem_delete");
        await packageItemBroker.DeletePackageItemAsync(entity: dataPackageItem);
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