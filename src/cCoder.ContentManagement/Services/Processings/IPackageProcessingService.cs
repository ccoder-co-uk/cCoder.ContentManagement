// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

public interface IPackageProcessingService
{
    Package GetPackage(Guid packageId);

    IQueryable<Package> GetAllPackage(bool ignoreFilters = false);

    ValueTask<Package> AddPackageAsync(Package newPackage);

    ValueTask<Package> UpdatePackageAsync(Package updatedPackage);

    ValueTask DeleteAsync(Guid packageId);

    ValueTask<IEnumerable<OperationResult<Package>>> AddOrUpdatePackageResult(IEnumerable<Package> newPackage);

    ValueTask DeleteAllPackageAsync(IEnumerable<Package> deletedPackage);
}