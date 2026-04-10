using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Exports;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Extensions;
using Package = cCoder.Data.Models.Packaging.Package;
using PackageItem = cCoder.Data.Models.Packaging.PackageItem;

namespace cCoder.ContentManagement.Services.Processings;

internal class PackageProcessingService(
    IPackageService service,
    IPackageExportService packageExportService,
    IPackageItemProcessingService packageItemService) : IPackageProcessingService
{
    public Package ExportPackage(int appId, string packageName)
    {
        string text = ValidatePackageName(packageName, "packageName");
        if (1 == 0)
        {
        }
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
            _ => new Package(packageName)
            {
                Items = Array.Empty<PackageItem>()
            },
        };
        if (1 == 0)
        {
        }
        return result;
    }

    public Package[] ExportPackages(int appId, string[] packageNames)
    {
        return (from name in ValidatePackageNames(packageNames, "packageNames")
                select ExportPackage(appId, name)).ToArray();
    }

    public Package Get(Guid id)
    {
        return service.Get(ValidateId(id, "id"));
    }

    public IQueryable<Package> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters);
    }

    public ValueTask<Package> AddAsync(Package entity)
    {
        ValidatePackage(entity, "entity");
        if (entity.Items != null && entity.Items.Any())
        {
            entity.Items.ForEach(delegate (PackageItem item)
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
            await packageItemService.DeleteAllAsync((from i in packageItemService.GetAll()
                                                     where i.PackageId == result.Id
                                                     select i).ToArray());
            entity.Items.ForEach(delegate (PackageItem i)
            {
                i.PackageId = result.Id;
            });
            await packageItemService.AddOrUpdate(entity.Items);
        }
        return result;
    }

    public ValueTask DeleteAsync(Guid id)
    {
        return service.DeleteAsync(ValidateId(id, "id"));
    }

    public async ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<Package>>> AddOrUpdate(IEnumerable<Package> items)
    {
        ValidatePackages(items, "items");
        List<cCoder.ContentManagement.Models.Result<Package>> results = new List<cCoder.ContentManagement.Models.Result<Package>>();
        foreach (Package item in items)
        {
            try
            {
                Package savedItem = item.Id == Guid.Empty ? await AddAsync(item) : await UpdateAsync(item);
                results.Add(new cCoder.ContentManagement.Models.Result<Package>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new cCoder.ContentManagement.Models.Result<Package>
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
        {
            await DeleteAsync(item.Id);
        }
    }

    private static int ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
        return appId;
    }

    private static Guid ValidateId(Guid id, string parameterName)
    {
        if (id == Guid.Empty)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return id;
    }

    private static Package ValidatePackage(Package package, string parameterName)
    {
        if (package == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return package;
    }

    private static IEnumerable<Package> ValidatePackages(IEnumerable<Package> packages, string parameterName)
    {
        if (packages == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return packages;
    }

    private static string ValidatePackageName(string packageName, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(packageName))
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return packageName;
    }

    private static string[] ValidatePackageNames(string[] packageNames, string parameterName)
    {
        if (packageNames == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return packageNames;
    }
}

