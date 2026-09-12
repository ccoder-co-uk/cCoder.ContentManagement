// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

internal interface IPackageImportEventBroker : IAuthenticatedEventBroker
{
    ValueTask RaiseImportAsync<T>(
        string eventName,
        EventMessage<PackageItemImportEvent<T>> message);
}