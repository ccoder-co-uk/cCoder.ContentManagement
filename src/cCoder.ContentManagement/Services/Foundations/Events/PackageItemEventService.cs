// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using DataPackageItem = cCoder.Data.Models.Packaging.PackageItem;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class PackageItemEventService(IPackageItemEventBroker packageItemEventBroker, ICoreAuthInfo authInfo) : IPackageItemEventService
{
    public async ValueTask RaisePackageItemAddEventAsync(PackageItem entity)
    {
        EventMessage<DataPackageItem> message = new EventMessage<DataPackageItem>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await packageItemEventBroker.RaisePackageItemAddEventAsync(message: message);
    }

    public async ValueTask RaisePackageItemUpdateEventAsync(PackageItem entity)
    {
        EventMessage<DataPackageItem> message = new EventMessage<DataPackageItem>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await packageItemEventBroker.RaisePackageItemUpdateEventAsync(message: message);
    }

    public async ValueTask RaisePackageItemDeleteEventAsync(PackageItem entity)
    {
        EventMessage<DataPackageItem> message = new EventMessage<DataPackageItem>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await packageItemEventBroker.RaisePackageItemDeleteEventAsync(message: message);
    }
}