// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using DataPackageItem = cCoder.Data.Models.Packaging.PackageItem;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class PackageItemEventService(IPackageItemEventBroker packageItemEventBroker) : IPackageItemEventService
{
    public ValueTask RaisePackageItemAddEventAsync(PackageItem packageItem) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePackageItemAddEventAsync(inputs: [packageItem]);

        EventMessage<DataPackageItem> message = new EventMessage<DataPackageItem>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = packageItemEventBroker.GetCurrentUserId()
            },
            Data = packageItem
        };

        await packageItemEventBroker.RaisePackageItemAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaisePackageItemUpdateEventAsync(PackageItem packageItem) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePackageItemUpdateEventAsync(inputs: [packageItem]);

        EventMessage<DataPackageItem> message = new EventMessage<DataPackageItem>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = packageItemEventBroker.GetCurrentUserId()
            },
            Data = packageItem
        };

        await packageItemEventBroker.RaisePackageItemUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaisePackageItemDeleteEventAsync(PackageItem packageItem) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePackageItemDeleteEventAsync(inputs: [packageItem]);

        EventMessage<DataPackageItem> message = new EventMessage<DataPackageItem>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = packageItemEventBroker.GetCurrentUserId()
            },
            Data = packageItem
        };

        await packageItemEventBroker.RaisePackageItemDeleteEventAsync(message: message);

    }, isValueTask: true);
}