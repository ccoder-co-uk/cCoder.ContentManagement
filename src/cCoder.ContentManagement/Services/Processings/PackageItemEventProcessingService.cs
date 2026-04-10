using cCoder.ContentManagement.Services.Foundations.Events;
using PackageItem = cCoder.Data.Models.Packaging.PackageItem;

namespace cCoder.ContentManagement.Services.Processings;

internal class PackageItemEventProcessingService(IPackageItemEventService eventService) : IPackageItemEventProcessingService
{
    public ValueTask RaisePackageItemAddEventAsync(PackageItem entity)
    {
        return eventService.RaisePackageItemAddEventAsync(entity);
    }

    public ValueTask RaisePackageItemUpdateEventAsync(PackageItem entity)
    {
        return eventService.RaisePackageItemUpdateEventAsync(entity);
    }

    public ValueTask RaisePackageItemDeleteEventAsync(PackageItem entity)
    {
        return eventService.RaisePackageItemDeleteEventAsync(entity);
    }
}
