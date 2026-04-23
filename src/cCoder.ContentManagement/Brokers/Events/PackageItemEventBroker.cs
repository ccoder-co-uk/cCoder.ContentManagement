using cCoder.Data.Models.Packaging;
using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public class PackageItemEventBroker(IEventHub eventHub) : IPackageItemEventBroker
{
    public ValueTask RaisePackageItemAddEventAsync(EventMessage<PackageItem> message)
    {
        return eventHub.RaiseEventAsync("package_item_add", message);
    }

    public ValueTask RaisePackageItemUpdateEventAsync(EventMessage<PackageItem> message)
    {
        return eventHub.RaiseEventAsync("package_item_update", message);
    }

    public ValueTask RaisePackageItemDeleteEventAsync(EventMessage<PackageItem> message)
    {
        return eventHub.RaiseEventAsync("package_item_delete", message);
    }
}
