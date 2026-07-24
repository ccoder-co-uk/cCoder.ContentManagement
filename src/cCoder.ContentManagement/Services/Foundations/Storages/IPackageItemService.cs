// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

public interface IPackageItemService
{
    PackageItem GetPackageItem(Guid packageItemId, bool ignoreFilters = false);

    IQueryable<PackageItem> GetAllPackageItem(bool ignoreFilters = false);

    ValueTask<PackageItem> AddPackageItemAsync(PackageItem newPackageItem);

    ValueTask<PackageItem> UpdatePackageItemAsync(PackageItem updatedPackageItem);

    ValueTask DeleteAsync(Guid packageItemId);
}