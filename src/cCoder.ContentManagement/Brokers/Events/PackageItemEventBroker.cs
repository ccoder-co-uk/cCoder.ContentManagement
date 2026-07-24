// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;
using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public class PackageItemEventBroker(IEventHub eventHub) : IPackageItemEventBroker
{
    public ValueTask RaisePackageItemAddEventAsync(EventMessage<PackageItem> message) =>
        eventHub.RaiseEventAsync(name: "package_item_add", message: message);

    public ValueTask RaisePackageItemUpdateEventAsync(EventMessage<PackageItem> message) =>
        eventHub.RaiseEventAsync(name: "package_item_update", message: message);

    public ValueTask RaisePackageItemDeleteEventAsync(EventMessage<PackageItem> message) =>
        eventHub.RaiseEventAsync(name: "package_item_delete", message: message);
}