// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.Packaging;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class PackageOrchestrationService(
    IContentManagementMigrationAggregationService contentManagementMigrationAggregationService,
    IPackageExportProcessingService packageExportProcessingService,
    IPackageProcessingService processingService,
    IPackageEventProcessingService eventService) : IPackageOrchestrationService
{
    public Package[] ExportPagackages(int appId, string[] packageNames) =>
        TryCatch<Package[]>(operation: () =>
    {
        ValidateExportPagackages(inputs: [appId, packageNames]);

        return ValidatePackageNames(packageNames: packageNames, parameterName: "packageNames")
            .Select(selector: packageName => packageExportProcessingService.ExportPackage(appId: appId, packageName: packageName))
            .ToArray();

    });

    public ValueTask ImportPackageAsync(int appId, Package package) =>
        TryCatch(operation: async () =>
    {
        ValidateImportPackageAsync(inputs: [appId, package]);
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidatePackage(package: package, parameterName: "package");
        await contentManagementMigrationAggregationService.ImportPackageAsync(appId: appId, package: package);

    }, isValueTask: true);

    public Package GetPackage(Guid packageId) =>
        TryCatch<Package>(operation: () =>
    {
        ValidatePackageOnGet(inputs: [packageId]);
        return processingService.GetPackage(packageId: ValidateId(packageId: packageId, parameterName: "id"));
    });

    public IQueryable<Package> GetAllPackage(bool ignoreFilters = false) =>
        TryCatch<IQueryable<Package>>(operation: () =>
    {
        ValidateAllPackageOnGet(inputs: [ignoreFilters]);
        return processingService.GetAllPackage(ignoreFilters: ignoreFilters);
    });

    public ValueTask<Package> AddPackageAsync(Package newPackage) =>
        TryCatch<Package>(operation: async () =>
    {
        ValidatePackageOnAdd(inputs: [newPackage]);
        ValidatePackage(package: newPackage, parameterName: "entity");

        Package result = await processingService.AddPackageAsync(newPackage: newPackage);
        await eventService.RaisePackageAddEventAsync(entity: result);
        return result;

    }, isValueTask: true);

    public ValueTask<Package> UpdatePackageAsync(Package updatedPackage) =>
        TryCatch<Package>(operation: async () =>
    {
        ValidatePackageOnUpdate(inputs: [updatedPackage]);
        ValidatePackage(package: updatedPackage, parameterName: "entity");

        Package result = await processingService.UpdatePackageAsync(updatedPackage: updatedPackage);
        await eventService.RaisePackageUpdateEventAsync(entity: result);
        return result;

    }, isValueTask: true);

    public ValueTask DeleteAsync(Guid packageId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [packageId]);
        ValidateId(packageId: packageId, parameterName: "id");

        Package entity = processingService.GetPackage(packageId: packageId);
        await eventService.RaisePackageDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(packageId: packageId);

    }, isValueTask: true);

    public ValueTask<IEnumerable<Result<Package>>> AddOrUpdatePackageResult(IEnumerable<Package> newPackage) =>
        TryCatch<IEnumerable<Result<Package>>>(operation: () =>
    {
        ValidateOrUpdatePackageResultOnAdd(inputs: [newPackage]);
        return processingService.AddOrUpdatePackageResult(newPackage: ValidatePackages(packages: newPackage, parameterName: "items"));
    }, isValueTask: true);

    public ValueTask DeleteAllPackageAsync(IEnumerable<Package> deletedPackage) =>
        TryCatch(operation: () =>
    {
        ValidateAllPackageOnDelete(inputs: [deletedPackage]);
        return processingService.DeleteAllPackageAsync(deletedPackage: ValidatePackages(packages: deletedPackage, parameterName: "items"));
    }, isValueTask: true);

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