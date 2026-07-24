// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.Packaging;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class PackageOrchestrationService(
    IPackageProcessingService processingService,
    IPackageItemProcessingService packageItemProcessingService,
    IPackageEventProcessingService eventService) : IPackageOrchestrationService
{
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
        await SynchronizePackageItemsAsync(updatedPackage: updatedPackage, packageId: result.Id);
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
        TryCatch<IEnumerable<Result<Package>>>(operation: async () =>
    {
        ValidateOrUpdatePackageResultOnAdd(inputs: [newPackage]);

        Package[] packages = ValidatePackages(packages: newPackage, parameterName: "items")
            .ToArray();

        bool[] existingPackages = packages
            .Select(selector: package => package.Id != Guid.Empty)
            .ToArray();

        Result<Package>[] results = (
            await processingService.AddOrUpdatePackageResult(newPackage: packages))
            .ToArray();

        for (int index = 0; index < packages.Length && index < results.Length; index++)
        {
            if (existingPackages[index] && results[index].Success)
            {
                await SynchronizePackageItemsAsync(
                    updatedPackage: packages[index],
                    packageId: results[index].Item.Id);
            }
        }

        return results;
    }, isValueTask: true);

    public ValueTask DeleteAllPackageAsync(IEnumerable<Package> deletedPackage) =>
        TryCatch(operation: () =>
    {
        ValidateAllPackageOnDelete(inputs: [deletedPackage]);
        return processingService.DeleteAllPackageAsync(deletedPackage: ValidatePackages(packages: deletedPackage, parameterName: "items"));
    }, isValueTask: true);

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

    private async ValueTask SynchronizePackageItemsAsync(
        Package updatedPackage,
        Guid packageId)
    {
        if (updatedPackage.Items == null)
        {
            return;
        }

        PackageItem[] deletedPackageItems = packageItemProcessingService
            .GetAllPackageItem()
            .Where(predicate: packageItem => packageItem.PackageId == packageId)
            .ToArray();

        await packageItemProcessingService.DeleteAllPackageItemAsync(
            deletedPackageItem: deletedPackageItems);

        foreach (PackageItem packageItem in updatedPackage.Items)
        {
            packageItem.PackageId = packageId;
        }

        if (updatedPackage.Items.Any())
        {
            await packageItemProcessingService.AddOrUpdatePackageItemResult(
                newPackageItem: updatedPackage.Items);
        }
    }
}