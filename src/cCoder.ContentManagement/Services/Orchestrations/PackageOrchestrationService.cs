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

    public Package GetPackage(Guid packageId) =>
        processingService.GetPackage(packageId: ValidateId(packageId: packageId, parameterName: "id"));

    public IQueryable<Package> GetAllPackage(bool ignoreFilters = false) =>
        processingService.GetAllPackage(ignoreFilters: ignoreFilters);

    public async ValueTask<Package> AddPackageAsync(Package newPackage)
    {
        ValidatePackage(package: newPackage, parameterName: "entity");

        Package result = await processingService.AddPackageAsync(newPackage: newPackage);
        await eventService.RaisePackageAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Package> UpdatePackageAsync(Package updatedPackage)
    {
        ValidatePackage(package: updatedPackage, parameterName: "entity");

        Package result = await processingService.UpdatePackageAsync(updatedPackage: updatedPackage);
        await eventService.RaisePackageUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(Guid packageId)
    {
        ValidateId(packageId: packageId, parameterName: "id");

        Package entity = processingService.GetPackage(packageId: packageId);
        await eventService.RaisePackageDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(packageId: packageId);
    }

    public ValueTask<IEnumerable<Result<Package>>> AddOrUpdatePackageResult(IEnumerable<Package> newPackage) =>
        processingService.AddOrUpdatePackageResult(newPackage: ValidatePackages(packages: newPackage, parameterName: "items"));

    public ValueTask DeleteAllPackageAsync(IEnumerable<Package> deletedPackage) =>
        processingService.DeleteAllPackageAsync(deletedPackage: ValidatePackages(packages: deletedPackage, parameterName: "items"));

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

    private static string[] ValidatePackageNames(string[] packageNames, string parameterName)
    {
        if (packageNames == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return packageNames;
    }
}