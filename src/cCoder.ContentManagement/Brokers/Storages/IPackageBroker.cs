// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IPackageBroker
{
    IQueryable<Package> GetAllPackages(bool ignoreFilters);

    ValueTask<Package> AddPackageAsync(Package newPackage);

    ValueTask<Package> UpdatePackageAsync(Package updatedPackage);

    ValueTask<int> DeletePackageAsync(Package deletedPackage);

    ValueTask DeleteAllPackagesAsync(IEnumerable<Package> deletedPackage);
}