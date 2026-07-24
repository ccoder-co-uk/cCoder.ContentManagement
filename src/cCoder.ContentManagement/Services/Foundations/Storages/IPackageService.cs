// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

public interface IPackageService
{
    Package GetPackage(Guid packageId, bool ignoreFilters = false);

    IQueryable<Package> GetAllPackage(bool ignoreFilters = false);

    ValueTask<Package> AddPackageAsync(Package newPackage);

    ValueTask<Package> UpdatePackageAsync(Package updatedPackage);

    ValueTask DeleteAsync(Guid packageId);
}