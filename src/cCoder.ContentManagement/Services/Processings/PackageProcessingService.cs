// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Extensions;
using cCoder.Data.Models.Packaging;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class PackageProcessingService(
    IPackageService service) : IPackageProcessingService
{
    public Package GetPackage(Guid packageId) =>
        TryCatch<Package>(operation: () =>
    {
        ValidatePackageOnGet(inputs: [packageId]);
        return service.GetPackage(packageId: ValidateId(packageId: packageId, parameterName: "id"));
    });

    public IQueryable<Package> GetAllPackage(bool ignoreFilters = false) =>
        TryCatch<IQueryable<Package>>(operation: () =>
    {
        ValidateAllPackageOnGet(inputs: [ignoreFilters]);
        return service.GetAllPackage(ignoreFilters: ignoreFilters);
    });

    public ValueTask<Package> AddPackageAsync(Package newPackage) =>
        TryCatch<Package>(operation: () =>
    {
        ValidatePackageOnAdd(inputs: [newPackage]);
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

    }, isValueTask: true);

    public ValueTask<Package> UpdatePackageAsync(Package updatedPackage) =>
        TryCatch<Package>(operation: async () =>
    {
        ValidatePackageOnUpdate(inputs: [updatedPackage]);
        ValidatePackage(package: updatedPackage, parameterName: "entity");
        return await service.UpdatePackageAsync(updatedPackage: updatedPackage);

    }, isValueTask: true);

    public ValueTask DeleteAsync(Guid packageId) =>
        TryCatch(operation: () =>
    {
        ValidateDeleteAsync(inputs: [packageId]);
        return service.DeleteAsync(packageId: ValidateId(packageId: packageId, parameterName: "id"));
    }, isValueTask: true);

    public ValueTask<IEnumerable<OperationResult<Package>>> AddOrUpdatePackageResult(IEnumerable<Package> newPackage) =>
        TryCatch<IEnumerable<OperationResult<Package>>>(operation: async () =>
    {
        ValidateOrUpdatePackageResultOnAdd(inputs: [newPackage]);
        ValidatePackages(packages: newPackage, parameterName: "items");
        List<OperationResult<Package>> results = new List<OperationResult<Package>>();

        foreach (Package item in newPackage)
        {
            try
            {
                Package savedItem = item.Id == Guid.Empty ? await ExecuteAddPackageAsync(newPackage: item) : await ExecuteUpdatePackageAsync(updatedPackage: item);

                results.Add(item: new OperationResult<Package>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new OperationResult<Package>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;

    }, isValueTask: true);

    public ValueTask DeleteAllPackageAsync(IEnumerable<Package> deletedPackage) =>
        TryCatch(operation: async () =>
    {
        ValidateAllPackageOnDelete(inputs: [deletedPackage]);
        ValidatePackages(packages: deletedPackage, parameterName: "items");

        foreach (Package item in deletedPackage)
        {
            await ExecuteDeleteAsync(packageId: item.Id);
        }

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

    private ValueTask<Package> ExecuteAddPackageAsync(Package newPackage)
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

    private ValueTask ExecuteDeleteAsync(Guid packageId) =>
        service.DeleteAsync(packageId: ValidateId(packageId: packageId, parameterName: "id"));

    private async ValueTask<Package> ExecuteUpdatePackageAsync(Package updatedPackage)
    {
        ValidatePackage(package: updatedPackage, parameterName: "entity");
        return await service.UpdatePackageAsync(updatedPackage: updatedPackage);
    }
}