using PackageItem = cCoder.Data.Models.Packaging.PackageItem;

namespace cCoder.ContentManagement.Services.Processings;

public interface IPackageItemEventProcessingService
{
    ValueTask RaisePackageItemAddEventAsync(PackageItem entity);

    ValueTask RaisePackageItemUpdateEventAsync(PackageItem entity);

    ValueTask RaisePackageItemDeleteEventAsync(PackageItem entity);
}
