// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;
using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public class PackageEventBroker(IEventHub eventHub) : IPackageEventBroker
{
    public ValueTask RaisePackageImportEventAsync(EventMessage<(int, Package)> message) =>
        eventHub.RaiseEventAsync(name: "package_import", message: message);

    public ValueTask RaisePackageAddEventAsync(EventMessage<Package> message) =>
        eventHub.RaiseEventAsync(name: "package_add", message: message);

    public ValueTask RaisePackageUpdateEventAsync(EventMessage<Package> message) =>
        eventHub.RaiseEventAsync(name: "package_update", message: message);

    public ValueTask RaisePackageDeleteEventAsync(EventMessage<Package> message) =>
        eventHub.RaiseEventAsync(name: "package_delete", message: message);
}