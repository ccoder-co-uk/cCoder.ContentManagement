// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public interface IPackageEventBroker : IAuthenticatedEventBroker
{
    ValueTask RaisePackageAddEventAsync(EventMessage<Package> message);

    ValueTask RaisePackageUpdateEventAsync(EventMessage<Package> message);

    ValueTask RaisePackageDeleteEventAsync(EventMessage<Package> message);
}