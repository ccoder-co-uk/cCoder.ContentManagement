using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Processings;

internal class PackageItemEventProcessingService(IPackageItemEventService eventService) : IPackageItemEventProcessingService
{
    public ValueTask RaisePackageItemAddEventAsync(PackageItem entity) =>
        eventService.RaisePackageItemAddEventAsync(entity);

    public ValueTask RaisePackageItemUpdateEventAsync(PackageItem entity) =>
        eventService.RaisePackageItemUpdateEventAsync(entity);

    public ValueTask RaisePackageItemDeleteEventAsync(PackageItem entity) =>
        eventService.RaisePackageItemDeleteEventAsync(entity);
}
