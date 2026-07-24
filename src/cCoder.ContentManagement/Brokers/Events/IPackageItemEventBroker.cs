// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public interface IPackageItemEventBroker : IAuthenticatedEventBroker
{
    ValueTask RaisePackageItemAddEventAsync(EventMessage<PackageItem> message);

    ValueTask RaisePackageItemUpdateEventAsync(EventMessage<PackageItem> message);

    ValueTask RaisePackageItemDeleteEventAsync(EventMessage<PackageItem> message);
}