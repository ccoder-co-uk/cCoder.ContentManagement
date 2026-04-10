using cCoder.ContentManagement.Services.Foundations.Events;
using Package = cCoder.Data.Models.Packaging.Package;

namespace cCoder.ContentManagement.Services.Processings;

internal class PackageEventProcessingService(IPackageEventService eventService) : IPackageEventProcessingService
{
    public ValueTask RaisePackageImportEvent(int appId, Package package)
    {
        return eventService.RaisePackageImportEventAsync(appId, package);
    }

    public ValueTask RaisePackageAddEventAsync(Package package)
    {
        return eventService.RaisePackageAddEventAsync(package);
    }

    public ValueTask RaisePackageUpdateEventAsync(Package package)
    {
        return eventService.RaisePackageUpdateEventAsync(package);
    }

    public ValueTask RaisePackageDeleteEventAsync(Package package)
    {
        return eventService.RaisePackageDeleteEventAsync(package);
    }
}
