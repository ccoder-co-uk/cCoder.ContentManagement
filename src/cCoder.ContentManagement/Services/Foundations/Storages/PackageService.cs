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
    public Package Get(Guid id, bool ignoreFilters = false)
    {
        ValidateId(id: id, parameterName: "id");

        if (ignoreFilters)
        {
            return GetAll(ignoreFilters: true)
                        .FirstOrDefault(predicate: (Package i) => i.Id == id);
        }

        Package package = GetAll()
            .FirstOrDefault(predicate: (Package i) => i.Id == id);

        if (package != null)
        {
            return package;
        }

        Package package2 = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: (Package i) => i.Id == id);

        if (package2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }

    public IQueryable<Package> GetAll(bool ignoreFilters = false) =>
        packageBroker.GetAllPackages(ignoreFilters: ignoreFilters);

    public async ValueTask<Package> AddAsync(Package package)
    {
        ValidatePackage(package: package, parameterName: "package");
        authorizationBroker.Authorize(appId: null, privilege: "Package_create");
        Package result = await packageBroker.AddPackageAsync(entity: CreateStoragePackage(package: package));
        package.Id = result.Id;
        package.Name = result.Name;
        package.Description = result.Description;
        package.Category = result.Category;
        package.SourceApi = result.SourceApi;
        return package;
    }

    public async ValueTask<Package> UpdateAsync(Package package)
    {
        ValidatePackage(package: package, parameterName: "package");
        authorizationBroker.Authorize(appId: null, privilege: "Package_update");
        Package result = await packageBroker.UpdatePackageAsync(entity: CreateStoragePackage(package: package));
        package.Id = result.Id;
        package.Name = result.Name;
        package.Description = result.Description;
        package.Category = result.Category;
        package.SourceApi = result.SourceApi;
        return package;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        ValidateId(id: id, parameterName: "id");
        Package package = Get(id: id);
        authorizationBroker.Authorize(appId: null, privilege: "Package_delete");
        await packageBroker.DeletePackageAsync(entity: CreateStoragePackage(package: package));
    }

    private static Package CreateStoragePackage(Package package)
    {
        if (package == null)
        {
            return null;
        }

        return new Package
        {
            Id = package.Id,
            Name = package.Name,
            Description = package.Description,
            Category = package.Category,
            SourceApi = package.SourceApi
        };
    }
}