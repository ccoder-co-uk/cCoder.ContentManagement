using cCoder.ContentManagement.Services.Processings;
using PackageItem = cCoder.Data.Models.Packaging.PackageItem;
using Result = cCoder.ContentManagement.Models.Result<cCoder.Data.Models.Packaging.PackageItem>;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class PackageItemOrchestrationService(
    IPackageItemProcessingService processingService,
    IPackageItemEventProcessingService eventService) : IPackageItemOrchestrationService
{
    public PackageItem Get(Guid id) => processingService.Get(id);

    public IQueryable<PackageItem> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters);

    public async ValueTask<PackageItem> AddAsync(PackageItem entity)
    {
        PackageItem result = await processingService.AddAsync(entity);
        await eventService.RaisePackageItemAddEventAsync(result);
        return result;
    }

    public async ValueTask<PackageItem> UpdateAsync(PackageItem entity)
    {
        PackageItem result = await processingService.UpdateAsync(entity);
        await eventService.RaisePackageItemUpdateEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        PackageItem entity = processingService.Get(id);
        await eventService.RaisePackageItemDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public ValueTask<IEnumerable<Result>> AddOrUpdate(IEnumerable<PackageItem> items) =>
        processingService.AddOrUpdate(items);

    public ValueTask DeleteAllAsync(IEnumerable<PackageItem> items) =>
        processingService.DeleteAllAsync(items);
}
