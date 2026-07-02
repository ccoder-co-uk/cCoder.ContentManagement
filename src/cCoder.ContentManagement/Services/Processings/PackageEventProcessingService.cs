using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Processings;

internal class PackageEventProcessingService(IPackageEventService eventService) : IPackageEventProcessingService
{
    public ValueTask RaisePackageImportEvent(int appId, Package package) =>
        eventService.RaisePackageImportEventAsync(appId, package);

    public ValueTask RaisePackageAddEventAsync(Package package) =>
        eventService.RaisePackageAddEventAsync(package);

    public ValueTask RaisePackageUpdateEventAsync(Package package) =>
        eventService.RaisePackageUpdateEventAsync(package);

    public ValueTask RaisePackageDeleteEventAsync(Package package) =>
        eventService.RaisePackageDeleteEventAsync(package);
}
