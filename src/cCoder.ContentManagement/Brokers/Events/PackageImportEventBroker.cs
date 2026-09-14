// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

internal sealed class PackageImportEventBroker(IEventHub eventHub)
    : IPackageImportEventBroker
{
    public ValueTask RaiseImportAsync<T>(
        string eventName,
        EventMessage<PackageItemImportEvent<T>> message) =>
        eventHub.RaiseEventAsync(name: eventName, message: message);
}