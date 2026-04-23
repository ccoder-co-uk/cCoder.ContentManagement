using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using Package = cCoder.Data.Models.Packaging.Package;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PackageService(IPackageBroker packageBroker, IAuthorizationBroker authorizationBroker) : IPackageService
{
    public Package Get(Guid id, bool ignoreFilters = false)
    {
        ValidateId(id, "id");
        if (ignoreFilters)
        {
            return GetAll(ignoreFilters: true).FirstOrDefault((Package i) => i.Id == id);
        }

        Package package = GetAll().FirstOrDefault((Package i) => i.Id == id);
        if (package != null)
        {
            return package;
        }
        Package package2 = GetAll(ignoreFilters: true).FirstOrDefault((Package i) => i.Id == id);
        if (package2 != null)
        {
            throw new SecurityException("Access Denied!");
        }
        return null;
    }

    public IQueryable<Package> GetAll(bool ignoreFilters = false)
    {
        return packageBroker.GetAllPackages(ignoreFilters);
    }

    public async ValueTask<Package> AddAsync(Package package)
    {
        ValidatePackage(package, "package");
        authorizationBroker.Authorize(null, "Package_create");
        Package result = await packageBroker.AddPackageAsync(CreateStoragePackage(package));
        package.Id = result.Id;
        package.Name = result.Name;
        package.Description = result.Description;
        package.Category = result.Category;
        package.SourceApi = result.SourceApi;
        return package;
    }

    public async ValueTask<Package> UpdateAsync(Package package)
    {
        ValidatePackage(package, "package");
        authorizationBroker.Authorize(null, "Package_update");
        Package result = await packageBroker.UpdatePackageAsync(CreateStoragePackage(package));
        package.Id = result.Id;
        package.Name = result.Name;
        package.Description = result.Description;
        package.Category = result.Category;
        package.SourceApi = result.SourceApi;
        return package;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        ValidateId(id, "id");
        Package package = Get(id);
        authorizationBroker.Authorize(null, "Package_delete");
        await packageBroker.DeletePackageAsync(CreateStoragePackage(package));
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
