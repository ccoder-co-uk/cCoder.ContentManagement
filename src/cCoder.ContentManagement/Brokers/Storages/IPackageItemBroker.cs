// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IPackageItemBroker
{
    IQueryable<PackageItem> GetAllPackageItems(bool ignoreFilters);

    ValueTask<PackageItem> AddPackageItemAsync(PackageItem newPackageItem);

    ValueTask<PackageItem> UpdatePackageItemAsync(PackageItem updatedPackageItem);

    ValueTask<int> DeletePackageItemAsync(PackageItem deletedPackageItem);

    ValueTask DeleteAllPackageItemsAsync(IEnumerable<PackageItem> deletedPackageItem);

    int? GetAppId(PackageItem entity);
}