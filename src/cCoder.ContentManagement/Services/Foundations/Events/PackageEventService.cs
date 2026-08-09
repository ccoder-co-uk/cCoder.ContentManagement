// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.ContentManagement.Models;
using cCoder.Data;
using cCoder.Eventing.Models;
using DataPackage = cCoder.Data.Models.Packaging.Package;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class PackageEventService(IPackageEventBroker packageEventBroker) : IPackageEventService
{
    public ValueTask RaisePackageImportEventAsync(int appId, Package package) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePackageImportEventAsync(inputs: [appId, package]);

        EventMessage<PackageImportEvent> message = new EventMessage<PackageImportEvent>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = packageEventBroker.GetCurrentUserId()
            },
            Data = new PackageImportEvent
            {
                AppId = appId,
                Package = package
            }
        };

        await packageEventBroker.RaisePackageImportEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaisePackageImportCompleteEventAsync(int appId, Package package) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePackageImportEventAsync(inputs: [appId, package]);

        EventMessage<PackageImportEvent> message = new EventMessage<PackageImportEvent>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = packageEventBroker.GetCurrentUserId()
            },
            Data = new PackageImportEvent
            {
                AppId = appId,
                Package = package
            }
        };

        await packageEventBroker.RaisePackageImportCompleteEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaisePackageAddEventAsync(Package package) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePackageAddEventAsync(inputs: [package]);

        EventMessage<DataPackage> message = new EventMessage<DataPackage>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = packageEventBroker.GetCurrentUserId()
            },
            Data = package
        };

        await packageEventBroker.RaisePackageAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaisePackageUpdateEventAsync(Package package) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePackageUpdateEventAsync(inputs: [package]);

        EventMessage<DataPackage> message = new EventMessage<DataPackage>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = packageEventBroker.GetCurrentUserId()
            },
            Data = package
        };

        await packageEventBroker.RaisePackageUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaisePackageDeleteEventAsync(Package package) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePackageDeleteEventAsync(inputs: [package]);

        EventMessage<DataPackage> message = new EventMessage<DataPackage>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = packageEventBroker.GetCurrentUserId()
            },
            Data = package
        };

        await packageEventBroker.RaisePackageDeleteEventAsync(message: message);

    }, isValueTask: true);
}