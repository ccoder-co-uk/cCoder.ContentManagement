using PackageItem = cCoder.Data.Models.Packaging.PackageItem;

namespace cCoder.ContentManagement.Services.Foundations.Events;

public interface IPackageItemEventService
{
    ValueTask RaisePackageItemAddEventAsync(PackageItem entity);

    ValueTask RaisePackageItemUpdateEventAsync(PackageItem entity);

    ValueTask RaisePackageItemDeleteEventAsync(PackageItem entity);
}

