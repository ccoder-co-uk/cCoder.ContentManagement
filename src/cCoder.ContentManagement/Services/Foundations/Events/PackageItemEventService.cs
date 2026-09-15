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
    public ValueTask RaisePackageItemAddEventAsync(PackageItem packageItem, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePackageItemAddEventAsync(inputs: [packageItem, userId]);

        EventMessage<DataPackageItem> message = new EventMessage<DataPackageItem>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = packageItem
        };

        await packageItemEventBroker.RaisePackageItemAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaisePackageItemUpdateEventAsync(PackageItem packageItem, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePackageItemUpdateEventAsync(inputs: [packageItem, userId]);

        EventMessage<DataPackageItem> message = new EventMessage<DataPackageItem>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = packageItem
        };

        await packageItemEventBroker.RaisePackageItemUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaisePackageItemDeleteEventAsync(PackageItem packageItem, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePackageItemDeleteEventAsync(inputs: [packageItem, userId]);

        EventMessage<DataPackageItem> message = new EventMessage<DataPackageItem>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = packageItem
        };

        await packageItemEventBroker.RaisePackageItemDeleteEventAsync(message: message);

    }, isValueTask: true);
}