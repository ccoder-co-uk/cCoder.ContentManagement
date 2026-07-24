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
            "Roles" => packageExportService.ExportRoles(appId: ValidateAppId(appId: appId, parameterName: "appId")),
            "Layouts" => packageExportService.ExportLayouts(appId: ValidateAppId(appId: appId, parameterName: "appId")),
            "Templates" => packageExportService.ExportTemplates(appId: ValidateAppId(appId: appId, parameterName: "appId")),
            "Components" => packageExportService.ExportComponents(appId: ValidateAppId(appId: appId, parameterName: "appId")),
            "Scripts" => packageExportService.ExportScripts(appId: ValidateAppId(appId: appId, parameterName: "appId")),
            "Resources" => packageExportService.ExportResources(appId: ValidateAppId(appId: appId, parameterName: "appId")),
            "Pages" => packageExportService.ExportPages(appId: ValidateAppId(appId: appId, parameterName: "appId")),
            "PageRoles" => packageExportService.ExportPageRoles(appId: ValidateAppId(appId: appId, parameterName: "appId")),
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

    public Package Get(Guid id) =>
        service.Get(id: ValidateId(id: id, parameterName: "id"));

    public IQueryable<Package> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters: ignoreFilters);

    public ValueTask<Package> AddAsync(Package entity)
    {
        ValidatePackage(package: entity, parameterName: "entity");

        if (entity.Items != null && entity.Items.Any())
        {
            entity.Items.ForEach(action: item =>
            {
                item.PackageId = entity.Id;
                item.Package = null;
            });
        }

        return service.AddAsync(package: entity);
    }

    public async ValueTask<Package> UpdateAsync(Package entity)
    {
        ValidatePackage(package: entity, parameterName: "entity");
        Package result = await service.UpdateAsync(package: entity);

        if (entity.Items != null)
        {
            await packageItemService.DeleteAllAsync(items: packageItemService.GetAll()
                .Where(predicate: item => item.PackageId == result.Id)
                .ToArray());

            entity.Items.ForEach(action: item => item.PackageId = result.Id);

            if (entity.Items.Any())
            {
                await packageItemService.AddOrUpdate(items: entity.Items);
            }
        }

        return result;
    }

    public ValueTask DeleteAsync(Guid id) =>
        service.DeleteAsync(id: ValidateId(id: id, parameterName: "id"));

    public async ValueTask<IEnumerable<Result<Package>>> AddOrUpdate(IEnumerable<Package> items)
    {
        ValidatePackages(packages: items, parameterName: "items");
        List<Result<Package>> results = new List<Result<Package>>();

        foreach (Package item in items)
        {
            try
            {
                Package savedItem = item.Id == Guid.Empty ? await AddAsync(entity: item) : await UpdateAsync(entity: item);

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

    public async ValueTask DeleteAllAsync(IEnumerable<Package> items)
    {
        ValidatePackages(packages: items, parameterName: "items");

        foreach (Package item in items)
        {
            await DeleteAsync(id: item.Id);
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

    private static Guid ValidateId(Guid id, string parameterName)
    {
        if (id == Guid.Empty)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return id;
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