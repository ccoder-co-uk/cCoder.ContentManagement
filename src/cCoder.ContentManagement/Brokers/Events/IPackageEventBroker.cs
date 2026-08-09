// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Data.Models.Packaging;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public interface IPackageEventBroker : IAuthenticatedEventBroker
{
    ValueTask RaisePackageImportEventAsync(EventMessage<PackageImportEvent> message);
    ValueTask RaisePackageImportCompleteEventAsync(EventMessage<PackageImportEvent> message);

    ValueTask RaisePackageAddEventAsync(EventMessage<Package> message);

    ValueTask RaisePackageUpdateEventAsync(EventMessage<Package> message);

    ValueTask RaisePackageDeleteEventAsync(EventMessage<Package> message);
}