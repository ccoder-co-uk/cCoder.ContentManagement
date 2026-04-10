using Package = cCoder.Data.Models.Packaging.Package;

namespace cCoder.ContentManagement.Services.Foundations.Events;

public interface IPackageEventService
{
    ValueTask RaisePackageImportEventAsync(int appId, Package package);

    ValueTask RaisePackageAddEventAsync(Package entity);

    ValueTask RaisePackageUpdateEventAsync(Package entity);

    ValueTask RaisePackageDeleteEventAsync(Package entity);
}

