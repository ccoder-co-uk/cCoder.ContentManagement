using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using EventLibrary.Models;
using DataPackageItem = cCoder.Data.Models.Packaging.PackageItem;
using PackageItem = cCoder.Data.Models.Packaging.PackageItem;

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
        await packageItemEventBroker.RaisePackageItemAddEventAsync(message);
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
        await packageItemEventBroker.RaisePackageItemUpdateEventAsync(message);
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
        await packageItemEventBroker.RaisePackageItemDeleteEventAsync(message);
    }
}

