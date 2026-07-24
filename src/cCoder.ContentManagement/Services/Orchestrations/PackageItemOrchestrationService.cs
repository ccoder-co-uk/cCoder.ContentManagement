// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.Packaging;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class PackageItemOrchestrationService(
    IPackageItemProcessingService processingService,
    IPackageItemEventProcessingService eventService) : IPackageItemOrchestrationService
{
    public PackageItem GetPackageItem(Guid packageItemId) =>
        TryCatch<PackageItem>(operation: () =>
    {
        ValidatePackageItemOnGet(inputs: [packageItemId]);
        return processingService.GetPackageItem(packageItemId: packageItemId);
    });

    public IQueryable<PackageItem> GetAllPackageItem(bool ignoreFilters = false) =>
        TryCatch<IQueryable<PackageItem>>(operation: () =>
    {
        ValidateAllPackageItemOnGet(inputs: [ignoreFilters]);
        return processingService.GetAllPackageItem(ignoreFilters: ignoreFilters);
    });

    public ValueTask<PackageItem> AddPackageItemAsync(PackageItem newPackageItem) =>
        TryCatch<PackageItem>(operation: async () =>
    {
        ValidatePackageItemOnAdd(inputs: [newPackageItem]);
        PackageItem result = await processingService.AddPackageItemAsync(newPackageItem: newPackageItem);
        await eventService.RaisePackageItemAddEventAsync(entity: result);
        return result;

    }, isValueTask: true);

    public ValueTask<PackageItem> UpdatePackageItemAsync(PackageItem updatedPackageItem) =>
        TryCatch<PackageItem>(operation: async () =>
    {
        ValidatePackageItemOnUpdate(inputs: [updatedPackageItem]);
        PackageItem result = await processingService.UpdatePackageItemAsync(updatedPackageItem: updatedPackageItem);
        await eventService.RaisePackageItemUpdateEventAsync(entity: result);
        return result;

    }, isValueTask: true);

    public ValueTask DeleteAsync(Guid packageItemId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [packageItemId]);
        PackageItem entity = processingService.GetPackageItem(packageItemId: packageItemId);
        await eventService.RaisePackageItemDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(packageItemId: packageItemId);

    }, isValueTask: true);

    public ValueTask<IEnumerable<OperationResult<PackageItem>>> AddOrUpdatePackageItemResult(IEnumerable<PackageItem> newPackageItem) =>
        TryCatch<IEnumerable<OperationResult<PackageItem>>>(operation: () =>
    {
        ValidateOrUpdatePackageItemResultOnAdd(inputs: [newPackageItem]);
        return processingService.AddOrUpdatePackageItemResult(newPackageItem: newPackageItem);
    }, isValueTask: true);

    public ValueTask DeleteAllPackageItemAsync(IEnumerable<PackageItem> deletedPackageItem) =>
        TryCatch(operation: () =>
    {
        ValidateAllPackageItemOnDelete(inputs: [deletedPackageItem]);
        return processingService.DeleteAllPackageItemAsync(deletedPackageItem: deletedPackageItem);
    }, isValueTask: true);
}