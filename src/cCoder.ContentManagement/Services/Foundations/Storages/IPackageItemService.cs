using PackageItem = cCoder.Data.Models.Packaging.PackageItem;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

public interface IPackageItemService
{
    PackageItem Get(Guid id, bool ignoreFilters = false);

    IQueryable<PackageItem> GetAll(bool ignoreFilters = false);

    ValueTask<PackageItem> AddAsync(PackageItem packageItem);

    ValueTask<PackageItem> UpdateAsync(PackageItem packageItem);

    ValueTask DeleteAsync(Guid id);
}
