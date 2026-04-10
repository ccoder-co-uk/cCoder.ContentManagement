using cCoder.Data.Models.Packaging;
using EventLibrary;
using EventLibrary.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public class PackageEventBroker(IEventHub eventHub) : IPackageEventBroker
{
    public ValueTask RaisePackageImportEventAsync(EventMessage<(int, Package)> message)
    {
        return eventHub.RaiseEventAsync("package_import", message);
    }

    public ValueTask RaisePackageAddEventAsync(EventMessage<Package> message)
    {
        return eventHub.RaiseEventAsync("package_add", message);
    }

    public ValueTask RaisePackageUpdateEventAsync(EventMessage<Package> message)
    {
        return eventHub.RaiseEventAsync("package_update", message);
    }

    public ValueTask RaisePackageDeleteEventAsync(EventMessage<Package> message)
    {
        return eventHub.RaiseEventAsync("package_delete", message);
    }
}
