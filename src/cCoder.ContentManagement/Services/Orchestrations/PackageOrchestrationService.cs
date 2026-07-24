// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.Packaging;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class PackageOrchestrationService(
    IContentManagementMigrationAggregationService contentManagementMigrationAggregationService,
    IPackageExportProcessingService packageExportProcessingService,
    IPackageProcessingService processingService,
    IPackageEventProcessingService eventService) : IPackageOrchestrationService
{
    public Package[] ExportPagackages(int appId, string[] packageNames)
    {
        return ValidatePackageNames(packageNames: packageNames, parameterName: "packageNames")
            .Select(selector: packageName => packageExportProcessingService.ExportPackage(appId: appId, packageName: packageName))
            .ToArray();
    }

    public async ValueTask ImportPackageAsync(int appId, Package package)
    {
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidatePackage(package: package, parameterName: "package");
        await contentManagementMigrationAggregationService.ImportPackageAsync(appId: appId, package: package);
    }

    public Package Get(Guid id) =>
        processingService.Get(id: ValidateId(id: id, parameterName: "id"));

    public IQueryable<Package> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters: ignoreFilters);

    public async ValueTask<Package> AddAsync(Package entity)
    {
        ValidatePackage(package: entity, parameterName: "entity");

        Package result = await processingService.AddAsync(entity: entity);
        await eventService.RaisePackageAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Package> UpdateAsync(Package entity)
    {
        ValidatePackage(package: entity, parameterName: "entity");

        Package result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaisePackageUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        ValidateId(id: id, parameterName: "id");

        Package entity = processingService.Get(id: id);
        await eventService.RaisePackageDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: id);
    }

    public ValueTask<IEnumerable<Result<Package>>> AddOrUpdate(IEnumerable<Package> items) =>
        processingService.AddOrUpdate(items: ValidatePackages(packages: items, parameterName: "items"));

    public ValueTask DeleteAllAsync(IEnumerable<Package> items) =>
        processingService.DeleteAllAsync(items: ValidatePackages(packages: items, parameterName: "items"));

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

    private static string[] ValidatePackageNames(string[] packageNames, string parameterName)
    {
        if (packageNames == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return packageNames;
    }
}