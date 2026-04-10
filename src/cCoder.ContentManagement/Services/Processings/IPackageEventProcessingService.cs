using Package = cCoder.Data.Models.Packaging.Package;

namespace cCoder.ContentManagement.Services.Processings;

public interface IPackageEventProcessingService
{
    ValueTask RaisePackageImportEvent(int appId, Package package);

    ValueTask RaisePackageAddEventAsync(Package entity);

    ValueTask RaisePackageUpdateEventAsync(Package entity);

    ValueTask RaisePackageDeleteEventAsync(Package entity);
}
