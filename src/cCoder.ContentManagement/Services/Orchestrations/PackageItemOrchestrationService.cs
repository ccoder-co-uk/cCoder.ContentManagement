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
    public PackageItem Get(Guid id) =>
        processingService.Get(id: id);

    public IQueryable<PackageItem> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters: ignoreFilters);

    public async ValueTask<PackageItem> AddAsync(PackageItem entity)
    {
        PackageItem result = await processingService.AddAsync(entity: entity);
        await eventService.RaisePackageItemAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<PackageItem> UpdateAsync(PackageItem entity)
    {
        PackageItem result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaisePackageItemUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        PackageItem entity = processingService.Get(id: id);
        await eventService.RaisePackageItemDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: id);
    }

    public ValueTask<IEnumerable<Result<PackageItem>>> AddOrUpdate(IEnumerable<PackageItem> items) =>
        processingService.AddOrUpdate(items: items);

    public ValueTask DeleteAllAsync(IEnumerable<PackageItem> items) =>
        processingService.DeleteAllAsync(items: items);
}