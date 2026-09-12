// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.ContentManagement.Models;
using cCoder.Data;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal sealed partial class PackageImportEventService(
    IPackageImportEventBroker packageImportEventBroker)
    : IPackageImportEventService
{
    public ValueTask RaiseImportAsync<T>(
        string eventName,
        PackageItemImportEvent<T> import) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseImportAsync(inputs: [eventName, import]);

        return packageImportEventBroker.RaiseImportAsync(
            eventName: eventName,
            message: new EventMessage<PackageItemImportEvent<T>>
            {
                AuthInfo = new EventAuthInfo
                {
                    SSOUserId = packageImportEventBroker.GetCurrentUserId()
                },
                Data = import
            });
    }, isValueTask: true);
}