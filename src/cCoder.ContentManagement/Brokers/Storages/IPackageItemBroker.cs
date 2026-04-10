using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IPackageItemBroker
{
    IQueryable<PackageItem> GetAllPackageItems(bool ignoreFilters);

    ValueTask<PackageItem> AddPackageItemAsync(PackageItem entity);

    ValueTask<PackageItem> UpdatePackageItemAsync(PackageItem entity);

    ValueTask<int> DeletePackageItemAsync(PackageItem entity);

    ValueTask DeleteAllPackageItemsAsync(IEnumerable<PackageItem> items);

    int? GetAppId(PackageItem entity);
}
