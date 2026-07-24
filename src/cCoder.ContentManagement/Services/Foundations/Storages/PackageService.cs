// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PackageService(IPackageBroker packageBroker, IAuthorizationBroker authorizationBroker) : IPackageService
{
    public Package GetPackage(Guid packageId, bool ignoreFilters = false)
    {
        ValidateId(packageId: packageId, parameterName: "id");

        if (ignoreFilters)
        {
            return GetAllPackage(ignoreFilters: true)
                .FirstOrDefault(predicate: (Package i) => i.Id == packageId);
        }

        Package package = GetAllPackage()
            .FirstOrDefault(predicate: (Package i) => i.Id == packageId);

        if (package != null)
        {
            return package;
        }

        Package package2 = GetAllPackage(ignoreFilters: true)
            .FirstOrDefault(predicate: (Package i) => i.Id == packageId);

        if (package2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }

    public IQueryable<Package> GetAllPackage(bool ignoreFilters = false) =>
        packageBroker.GetAllPackages(ignoreFilters: ignoreFilters);

    public async ValueTask<Package> AddPackageAsync(Package newPackage)
    {
        ValidatePackage(package: newPackage, parameterName: "package");
        authorizationBroker.Authorize(appId: null, privilege: "Package_create");
        Package result = await packageBroker.AddPackageAsync(newPackage: CreateStoragePackage(newPackage: newPackage));
        newPackage.Id = result.Id;
        newPackage.Name = result.Name;
        newPackage.Description = result.Description;
        newPackage.Category = result.Category;
        newPackage.SourceApi = result.SourceApi;
        return newPackage;
    }

    public async ValueTask<Package> UpdatePackageAsync(Package updatedPackage)
    {
        ValidatePackage(package: updatedPackage, parameterName: "package");
        authorizationBroker.Authorize(appId: null, privilege: "Package_update");
        Package result = await packageBroker.UpdatePackageAsync(updatedPackage: CreateStoragePackage(newPackage: updatedPackage));
        updatedPackage.Id = result.Id;
        updatedPackage.Name = result.Name;
        updatedPackage.Description = result.Description;
        updatedPackage.Category = result.Category;
        updatedPackage.SourceApi = result.SourceApi;
        return updatedPackage;
    }

    public async ValueTask DeleteAsync(Guid packageId)
    {
        ValidateId(packageId: packageId, parameterName: "id");
        Package package = GetPackage(packageId: packageId);
        authorizationBroker.Authorize(appId: null, privilege: "Package_delete");
        await packageBroker.DeletePackageAsync(deletedPackage: CreateStoragePackage(newPackage: package));
    }

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
}