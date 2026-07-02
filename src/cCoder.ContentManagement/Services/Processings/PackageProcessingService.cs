using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Exports;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Extensions;
using cCoder.Data.Models.Packaging;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal class PackageProcessingService(
    IPackageService service,
    IPackageExportService packageExportService,
    IPackageItemProcessingService packageItemService) : IPackageProcessingService
{
    public Package ExportPackage(int appId, string packageName)
    {
        string text = ValidatePackageName(packageName, "packageName");
        Package result = text switch
        {
            "Roles" => packageExportService.ExportRoles(ValidateAppId(appId, "appId")),
            "Layouts" => packageExportService.ExportLayouts(ValidateAppId(appId, "appId")),
            "Templates" => packageExportService.ExportTemplates(ValidateAppId(appId, "appId")),
            "Components" => packageExportService.ExportComponents(ValidateAppId(appId, "appId")),
            "Scripts" => packageExportService.ExportScripts(ValidateAppId(appId, "appId")),
            "Resources" => packageExportService.ExportResources(ValidateAppId(appId, "appId")),
            "Pages" => packageExportService.ExportPages(ValidateAppId(appId, "appId")),
            "PageRoles" => packageExportService.ExportPageRoles(ValidateAppId(appId, "appId")),
            var ignoredPackage => new Package(packageName)
            {
                Items = Array.Empty<PackageItem>()
            },
        };
        return result;
    }

    public Package[] ExportPackages(int appId, string[] packageNames)
    {
        return ValidatePackageNames(packageNames, "packageNames")
            .Select(name => ExportPackage(appId, name))
            .ToArray();
    }

    public Package Get(Guid id) =>
        service.Get(ValidateId(id, "id"));

    public IQueryable<Package> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters);

    public ValueTask<Package> AddAsync(Package entity)
    {
        ValidatePackage(entity, "entity");
        if (entity.Items != null && entity.Items.Any())
        {
            entity.Items.ForEach(item =>
            {
                item.PackageId = entity.Id;
                item.Package = null;
            });
        }
        return service.AddAsync(entity);
    }

    public async ValueTask<Package> UpdateAsync(Package entity)
    {
        ValidatePackage(entity, "entity");
        Package result = await service.UpdateAsync(entity);
        if (entity.Items != null && entity.Items.Any())
        {
            await packageItemService.DeleteAllAsync(packageItemService.GetAll()
                .Where(item => item.PackageId == result.Id)
                .ToArray());

            entity.Items.ForEach(item =>
            {
                item.PackageId = result.Id;
            });

            await packageItemService.AddOrUpdate(entity.Items);
        }
        return result;
    }

    public ValueTask DeleteAsync(Guid id) =>
        service.DeleteAsync(ValidateId(id, "id"));

    public async ValueTask<IEnumerable<Result<Package>>> AddOrUpdate(IEnumerable<Package> items)
    {
        ValidatePackages(items, "items");
        List<Result<Package>> results = new List<Result<Package>>();
        foreach (Package item in items)
        {
            try
            {
                Package savedItem = item.Id == Guid.Empty ? await AddAsync(item) : await UpdateAsync(item);
                results.Add(new Result<Package>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result<Package>
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
        ValidatePackages(items, "items");
        foreach (Package item in items)
            await DeleteAsync(item.Id);
    }

    private static int ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
            throw new ValidationException(parameterName + " must be greater than 0.");

        return appId;
    }

    private static Guid ValidateId(Guid id, string parameterName)
    {
        if (id == Guid.Empty)
            throw new ValidationException(parameterName + " is required.");

        return id;
    }

    private static Package ValidatePackage(Package package, string parameterName)
    {
        if (package == null)
            throw new ValidationException(parameterName + " is required.");

        return package;
    }

    private static IEnumerable<Package> ValidatePackages(IEnumerable<Package> packages, string parameterName)
    {
        if (packages == null)
            throw new ValidationException(parameterName + " is required.");

        return packages;
    }

    private static string ValidatePackageName(string packageName, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(packageName))
            throw new ValidationException(parameterName + " is required.");

        return packageName;
    }

    private static string[] ValidatePackageNames(string[] packageNames, string parameterName)
    {
        if (packageNames == null)
            throw new ValidationException(parameterName + " is required.");

        return packageNames;
    }
}
