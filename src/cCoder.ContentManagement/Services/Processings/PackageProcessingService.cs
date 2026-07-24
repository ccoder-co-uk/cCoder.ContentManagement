// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Exports;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Extensions;
using cCoder.Data.Models.Packaging;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal class PackageProcessingService(
    IPackageService service,
    IPackageItemProcessingService packageItemService,
    IPackageExportService packageExportService) : IPackageProcessingService
{
    public Package ExportPackage(int appId, string packageName)
    {
        string text = ValidatePackageName(packageName: packageName, parameterName: "packageName");

        Package result = text switch
        {
            "Roles" => packageExportService.ExportRolesPackage(appId: ValidateAppId(appId: appId, parameterName: "appId")),
            "Layouts" => packageExportService.ExportLayoutsPackage(appId: ValidateAppId(appId: appId, parameterName: "appId")),
            "Templates" => packageExportService.ExportTemplatesPackage(appId: ValidateAppId(appId: appId, parameterName: "appId")),
            "Components" => packageExportService.ExportComponentsPackage(appId: ValidateAppId(appId: appId, parameterName: "appId")),
            "Scripts" => packageExportService.ExportScriptsPackage(appId: ValidateAppId(appId: appId, parameterName: "appId")),
            "Resources" => packageExportService.ExportResourcesPackage(appId: ValidateAppId(appId: appId, parameterName: "appId")),
            "Pages" => packageExportService.ExportPagesPackage(appId: ValidateAppId(appId: appId, parameterName: "appId")),
            "PageRoles" => packageExportService.ExportPageRolesPackage(appId: ValidateAppId(appId: appId, parameterName: "appId")),
            var ignoredPackage => new Package(name: packageName)
            {
                Items = Array.Empty<PackageItem>()
            },
        };

        return result;
    }

    public Package[] ExportPackages(int appId, string[] packageNames)
    {
        return ValidatePackageNames(packageNames: packageNames, parameterName: "packageNames")
            .Select(selector: name => ExportPackage(appId: appId, packageName: name))
            .ToArray();
    }

    public Package GetPackage(Guid packageId) =>
        service.GetPackage(packageId: ValidateId(packageId: packageId, parameterName: "id"));

    public IQueryable<Package> GetAllPackage(bool ignoreFilters = false) =>
        service.GetAllPackage(ignoreFilters: ignoreFilters);

    public ValueTask<Package> AddPackageAsync(Package newPackage)
    {
        ValidatePackage(package: newPackage, parameterName: "entity");

        if (newPackage.Items != null && newPackage.Items.Any())
        {
            newPackage.Items.ForEach(action: item =>
            {
                item.PackageId = newPackage.Id;
                item.Package = null;
            });
        }

        return service.AddPackageAsync(newPackage: newPackage);
    }

    public async ValueTask<Package> UpdatePackageAsync(Package updatedPackage)
    {
        ValidatePackage(package: updatedPackage, parameterName: "entity");
        Package result = await service.UpdatePackageAsync(updatedPackage: updatedPackage);

        if (updatedPackage.Items != null)
        {
            await packageItemService.DeleteAllPackageItemAsync(deletedPackageItem: packageItemService.GetAllPackageItem()
                .Where(predicate: item => item.PackageId == result.Id)
                .ToArray());

            updatedPackage.Items.ForEach(action: item => item.PackageId = result.Id);

            if (updatedPackage.Items.Any())
            {
                await packageItemService.AddOrUpdatePackageItemResult(newPackageItem: updatedPackage.Items);
            }
        }

        return result;
    }

    public ValueTask DeleteAsync(Guid packageId) =>
        service.DeleteAsync(packageId: ValidateId(packageId: packageId, parameterName: "id"));

    public async ValueTask<IEnumerable<Result<Package>>> AddOrUpdatePackageResult(IEnumerable<Package> newPackage)
    {
        ValidatePackages(packages: newPackage, parameterName: "items");
        List<Result<Package>> results = new List<Result<Package>>();

        foreach (Package item in newPackage)
        {
            try
            {
                Package savedItem = item.Id == Guid.Empty ? await AddPackageAsync(newPackage: item) : await UpdatePackageAsync(updatedPackage: item);

                results.Add(item: new Result<Package>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<Package>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask DeleteAllPackageAsync(IEnumerable<Package> deletedPackage)
    {
        ValidatePackages(packages: deletedPackage, parameterName: "items");

        foreach (Package item in deletedPackage)
        {
            await DeleteAsync(packageId: item.Id);
        }
    }

    private static int ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
        {
            throw new ValidationException(message: parameterName + " must be greater than 0.");
        }

        return appId;
    }

    private static Guid ValidateId(Guid packageId, string parameterName)
    {
        if (packageId == Guid.Empty)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return packageId;
    }

    private static Package ValidatePackage(Package package, string parameterName)
    {
        if (package == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return package;
    }

    private static IEnumerable<Package> ValidatePackages(IEnumerable<Package> packages, string parameterName)
    {
        if (packages == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return packages;
    }

    private static string ValidatePackageName(string packageName, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value: packageName))
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return packageName;
    }

    private static string[] ValidatePackageNames(string[] packageNames, string parameterName)
    {
        if (packageNames == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return packageNames;
    }
}