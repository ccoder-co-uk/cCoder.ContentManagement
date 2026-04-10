using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using EventLibrary.Models;
using DataPackage = cCoder.Data.Models.Packaging.Package;
using Package = cCoder.Data.Models.Packaging.Package;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class PackageEventService(IPackageEventBroker packageEventBroker, ICoreAuthInfo authInfo) : IPackageEventService
{
    public async ValueTask RaisePackageImportEventAsync(int appId, Package package)
    {
        EventMessage<(int, DataPackage)> message = new EventMessage<(int, DataPackage)>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = (appId, package)
        };
        await packageEventBroker.RaisePackageImportEventAsync(message);
    }

    public async ValueTask RaisePackageAddEventAsync(Package package)
    {
        EventMessage<DataPackage> message = new EventMessage<DataPackage>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = package
        };
        await packageEventBroker.RaisePackageAddEventAsync(message);
    }

    public async ValueTask RaisePackageUpdateEventAsync(Package package)
    {
        EventMessage<DataPackage> message = new EventMessage<DataPackage>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = package
        };
        await packageEventBroker.RaisePackageUpdateEventAsync(message);
    }

    public async ValueTask RaisePackageDeleteEventAsync(Package package)
    {
        EventMessage<DataPackage> message = new EventMessage<DataPackage>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = package
        };
        await packageEventBroker.RaisePackageDeleteEventAsync(message);
    }
}

