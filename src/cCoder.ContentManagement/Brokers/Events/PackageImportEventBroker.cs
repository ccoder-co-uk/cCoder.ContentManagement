// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

internal sealed class PackageImportEventBroker(IAuthenticatedEventHub eventHub)
    : AuthenticatedEventBroker(eventHub), IPackageImportEventBroker
{
    public ValueTask RaiseImportAsync<T>(
        string eventName,
        EventMessage<PackageItemImportEvent<T>> message) =>
        RaiseEventAsync(name: eventName, message: message);
}