// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.Packaging;

using cCoder.ContentManagement.Exposures;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PackageService(IPackageBroker packageBroker, IAuthorizationManager authorizationManager) : IPackageService
{
    public Package GetPackage(Guid packageId, bool ignoreFilters = false) =>
        TryCatch<Package>(operation: () =>
    {
        ValidatePackageOnGet(inputs: [packageId, ignoreFilters]);
        ValidateId(packageId: packageId, parameterName: "id");

        if (ignoreFilters)
        {
            return ExecuteGetAllPackage(ignoreFilters: true)
                .FirstOrDefault(predicate: (Package i) => i.Id == packageId);
        }

        Package package = ExecuteGetAllPackage()
            .FirstOrDefault(predicate: (Package i) => i.Id == packageId);

        if (package != null)
        {
            return package;
        }

        Package package2 = ExecuteGetAllPackage(ignoreFilters: true)
            .FirstOrDefault(predicate: (Package i) => i.Id == packageId);

        if (package2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;

    });

    public IQueryable<Package> GetAllPackage(bool ignoreFilters = false) =>
        TryCatch<IQueryable<Package>>(operation: () =>
    {
        ValidateAllPackageOnGet(inputs: [ignoreFilters]);

        return ignoreFilters
            ? packageBroker.GetAllPackagesIgnoringFilters()
            : packageBroker.GetAllPackages();
    });

    public ValueTask<Package> AddPackageAsync(Package newPackage) =>
        TryCatch<Package>(operation: async () =>
    {
        ValidatePackageOnAdd(inputs: [newPackage]);
        ValidatePackage(package: newPackage, parameterName: "package");
        authorizationManager.Authorize(appId: null, privilege: "Package_create");
        Package result = await packageBroker.AddPackageAsync(newPackage: CreateStoragePackage(newPackage: newPackage));
        newPackage.Id = result.Id;
        newPackage.Name = result.Name;
        newPackage.Description = result.Description;
        newPackage.Category = result.Category;
        newPackage.SourceApi = result.SourceApi;
        return newPackage;

    }, isValueTask: true);

    public ValueTask<Package> UpdatePackageAsync(Package updatedPackage) =>
        TryCatch<Package>(operation: async () =>
    {
        ValidatePackageOnUpdate(inputs: [updatedPackage]);
        ValidatePackage(package: updatedPackage, parameterName: "package");
        authorizationManager.Authorize(appId: null, privilege: "Package_update");
        Package result = await packageBroker.UpdatePackageAsync(updatedPackage: CreateStoragePackage(newPackage: updatedPackage));
        updatedPackage.Id = result.Id;
        updatedPackage.Name = result.Name;
        updatedPackage.Description = result.Description;
        updatedPackage.Category = result.Category;
        updatedPackage.SourceApi = result.SourceApi;
        return updatedPackage;

    }, isValueTask: true);

    public ValueTask DeleteAsync(Guid packageId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [packageId]);
        ValidateId(packageId: packageId, parameterName: "id");
        Package package = ExecuteGetPackage(packageId: packageId);
        authorizationManager.Authorize(appId: null, privilege: "Package_delete");
        await packageBroker.DeletePackageAsync(deletedPackage: CreateStoragePackage(newPackage: package));

    }, isValueTask: true);

    private static Package CreateStoragePackage(Package newPackage)
    {
        if (newPackage == null)
        {
            return null;
        }

        return new Package
        {
            Id = newPackage.Id,
            Name = newPackage.Name,
            Description = newPackage.Description,
            Category = newPackage.Category,
            SourceApi = newPackage.SourceApi
        };
    }

    private IQueryable<Package> ExecuteGetAllPackage(bool ignoreFilters = false) =>
        (ignoreFilters
            ? packageBroker.GetAllPackagesIgnoringFilters()
            : packageBroker.GetAllPackages());

    private Package ExecuteGetPackage(Guid packageId, bool ignoreFilters = false)
    {
        ValidateId(packageId: packageId, parameterName: "id");

        if (ignoreFilters)
        {
            return ExecuteGetAllPackage(ignoreFilters: true)
                .FirstOrDefault(predicate: (Package i) => i.Id == packageId);
        }

        Package package = ExecuteGetAllPackage()
            .FirstOrDefault(predicate: (Package i) => i.Id == packageId);

        if (package != null)
        {
            return package;
        }

        Package package2 = ExecuteGetAllPackage(ignoreFilters: true)
            .FirstOrDefault(predicate: (Package i) => i.Id == packageId);

        if (package2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }
}