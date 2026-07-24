// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IPackageItemOrchestrationService
{
    PackageItem GetPackageItem(Guid packageItemId);

    IQueryable<PackageItem> GetAllPackageItem(bool ignoreFilters = false);

    ValueTask<PackageItem> AddPackageItemAsync(PackageItem newPackageItem);

    ValueTask<PackageItem> UpdatePackageItemAsync(PackageItem updatedPackageItem);

    ValueTask DeleteAsync(Guid packageItemId);

    ValueTask<IEnumerable<Result<PackageItem>>> AddOrUpdatePackageItemResult(IEnumerable<PackageItem> newPackageItem);

    ValueTask DeleteAllPackageItemAsync(IEnumerable<PackageItem> deletedPackageItem);
}