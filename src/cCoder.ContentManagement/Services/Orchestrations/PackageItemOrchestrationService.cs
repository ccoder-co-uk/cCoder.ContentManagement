// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.Packaging;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class PackageItemOrchestrationService(
    IPackageItemProcessingService processingService,
    IPackageItemEventProcessingService eventService) : IPackageItemOrchestrationService
{
    public PackageItem GetPackageItem(Guid packageItemId) =>
        processingService.GetPackageItem(packageItemId: packageItemId);

    public IQueryable<PackageItem> GetAllPackageItem(bool ignoreFilters = false) =>
        processingService.GetAllPackageItem(ignoreFilters: ignoreFilters);

    public async ValueTask<PackageItem> AddPackageItemAsync(PackageItem newPackageItem)
    {
        PackageItem result = await processingService.AddPackageItemAsync(newPackageItem: newPackageItem);
        await eventService.RaisePackageItemAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<PackageItem> UpdatePackageItemAsync(PackageItem updatedPackageItem)
    {
        PackageItem result = await processingService.UpdatePackageItemAsync(updatedPackageItem: updatedPackageItem);
        await eventService.RaisePackageItemUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(Guid packageItemId)
    {
        PackageItem entity = processingService.GetPackageItem(packageItemId: packageItemId);
        await eventService.RaisePackageItemDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(packageItemId: packageItemId);
    }

    public ValueTask<IEnumerable<Result<PackageItem>>> AddOrUpdatePackageItemResult(IEnumerable<PackageItem> newPackageItem) =>
        processingService.AddOrUpdatePackageItemResult(newPackageItem: newPackageItem);

    public ValueTask DeleteAllPackageItemAsync(IEnumerable<PackageItem> deletedPackageItem) =>
        processingService.DeleteAllPackageItemAsync(deletedPackageItem: deletedPackageItem);
}