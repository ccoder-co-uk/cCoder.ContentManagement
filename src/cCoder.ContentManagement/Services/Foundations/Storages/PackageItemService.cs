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
    public PackageItem GetPackageItem(Guid packageItemId, bool ignoreFilters = false) =>
        TryCatch<PackageItem>(operation: () =>
    {
        ValidatePackageItemOnGet(inputs: [packageItemId, ignoreFilters]);

        if (ignoreFilters)
        {
            return ExecuteGetAllPackageItem(ignoreFilters: true)
                .FirstOrDefault(predicate: (PackageItem i) => i.Id == packageItemId);
        }

        PackageItem packageItem = ExecuteGetAllPackageItem()
            .FirstOrDefault(predicate: (PackageItem i) => i.Id == packageItemId);

        if (packageItem != null)
        {
            return packageItem;
        }

        PackageItem packageItem2 = ExecuteGetAllPackageItem(ignoreFilters: true)
            .FirstOrDefault(predicate: (PackageItem i) => i.Id == packageItemId);

        if (packageItem2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;

    });

    public IQueryable<PackageItem> GetAllPackageItem(bool ignoreFilters = false) =>
        TryCatch<IQueryable<PackageItem>>(operation: () =>
    {
        ValidateAllPackageItemOnGet(inputs: [ignoreFilters]);
        return packageItemBroker.GetAllPackageItems(ignoreFilters: ignoreFilters);
    });

    public ValueTask<PackageItem> AddPackageItemAsync(PackageItem newPackageItem) =>
        TryCatch<PackageItem>(operation: async () =>
    {
        ValidatePackageItemOnAdd(inputs: [newPackageItem]);
        PackageItem dataPackageItem = CreateStoragePackageItem(newPackageItem: newPackageItem);
        authorizationBroker.Authorize(appId: packageItemBroker.GetAppId(entity: dataPackageItem), privilege: "PackageItem_create");
        PackageItem result = await packageItemBroker.AddPackageItemAsync(newPackageItem: dataPackageItem);
        newPackageItem.Id = result.Id;
        newPackageItem.PackageId = result.PackageId;
        newPackageItem.Type = result.Type;
        newPackageItem.Data = result.Data;
        return newPackageItem;

    }, isValueTask: true);

    public ValueTask<PackageItem> UpdatePackageItemAsync(PackageItem updatedPackageItem) =>
        TryCatch<PackageItem>(operation: async () =>
    {
        ValidatePackageItemOnUpdate(inputs: [updatedPackageItem]);
        PackageItem dataPackageItem = CreateStoragePackageItem(newPackageItem: updatedPackageItem);
        authorizationBroker.Authorize(appId: packageItemBroker.GetAppId(entity: dataPackageItem), privilege: "PackageItem_update");
        PackageItem result = await packageItemBroker.UpdatePackageItemAsync(updatedPackageItem: dataPackageItem);
        updatedPackageItem.Id = result.Id;
        updatedPackageItem.PackageId = result.PackageId;
        updatedPackageItem.Type = result.Type;
        updatedPackageItem.Data = result.Data;
        return updatedPackageItem;

    }, isValueTask: true);

    public ValueTask DeleteAsync(Guid packageItemId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [packageItemId]);
        PackageItem packageItem = ExecuteGetPackageItem(packageItemId: packageItemId);
        PackageItem dataPackageItem = CreateStoragePackageItem(newPackageItem: packageItem);
        authorizationBroker.Authorize(appId: packageItemBroker.GetAppId(entity: dataPackageItem), privilege: "PackageItem_delete");
        await packageItemBroker.DeletePackageItemAsync(deletedPackageItem: dataPackageItem);

    }, isValueTask: true);

    private static PackageItem CreateStoragePackageItem(PackageItem newPackageItem)
    {
        if (newPackageItem == null)
        {
            return null;
        }

        return new PackageItem
        {
            Id = newPackageItem.Id,
            PackageId = newPackageItem.PackageId,
            Type = newPackageItem.Type,
            Data = newPackageItem.Data
        };
    }

    private IQueryable<PackageItem> ExecuteGetAllPackageItem(bool ignoreFilters = false) =>
        packageItemBroker.GetAllPackageItems(ignoreFilters: ignoreFilters);

    private PackageItem ExecuteGetPackageItem(Guid packageItemId, bool ignoreFilters = false)
    {
        if (ignoreFilters)
        {
            return ExecuteGetAllPackageItem(ignoreFilters: true)
                .FirstOrDefault(predicate: (PackageItem i) => i.Id == packageItemId);
        }

        PackageItem packageItem = ExecuteGetAllPackageItem()
            .FirstOrDefault(predicate: (PackageItem i) => i.Id == packageItemId);

        if (packageItem != null)
        {
            return packageItem;
        }

        PackageItem packageItem2 = ExecuteGetAllPackageItem(ignoreFilters: true)
            .FirstOrDefault(predicate: (PackageItem i) => i.Id == packageItemId);

        if (packageItem2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }
}