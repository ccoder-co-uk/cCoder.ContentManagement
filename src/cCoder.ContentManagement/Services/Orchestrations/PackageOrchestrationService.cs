using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Processings;
using Package = cCoder.Data.Models.Packaging.Package;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class PackageOrchestrationService(
    IContentManagementMigrationAggregationService contentManagementMigrationAggregationService,
    IPackageExportProcessingService packageExportProcessingService,
    IPackageProcessingService processingService,
    IPackageEventProcessingService eventService) : IPackageOrchestrationService
{
    public Package[] ExportPagackages(int appId, string[] packageNames)
    {
        return ValidatePackageNames(packageNames, "packageNames")
            .Select(packageName => packageExportProcessingService.ExportPackage(appId, packageName))
            .ToArray();
    }

    public async ValueTask ImportPackageAsync(int appId, Package package)
    {
        ValidateAppId(appId, "appId");
        ValidatePackage(package, "package");
        await contentManagementMigrationAggregationService.ImportPackageAsync(appId, package);
    }

    public Package Get(Guid id) => processingService.Get(ValidateId(id, "id"));

    public IQueryable<Package> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters);

    public async ValueTask<Package> AddAsync(Package entity)
    {
        ValidatePackage(entity, "entity");

        Package result = await processingService.AddAsync(entity);
        await eventService.RaisePackageAddEventAsync(result);
        return result;
    }

    public async ValueTask<Package> UpdateAsync(Package entity)
    {
        ValidatePackage(entity, "entity");

        Package result = await processingService.UpdateAsync(entity);
        await eventService.RaisePackageUpdateEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        ValidateId(id, "id");

        Package entity = processingService.Get(id);
        await eventService.RaisePackageDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<Package>>> AddOrUpdate(IEnumerable<Package> items) =>
        processingService.AddOrUpdate(ValidatePackages(items, "items"));

    public ValueTask DeleteAllAsync(IEnumerable<Package> items) =>
        processingService.DeleteAllAsync(ValidatePackages(items, "items"));

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

    private static string[] ValidatePackageNames(string[] packageNames, string parameterName)
    {
        if (packageNames == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }

        return packageNames;
    }
}
