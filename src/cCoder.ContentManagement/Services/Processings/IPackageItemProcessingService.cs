using PackageItem = cCoder.Data.Models.Packaging.PackageItem;

namespace cCoder.ContentManagement.Services.Processings;

public interface IPackageItemProcessingService
{
    PackageItem Get(Guid id);

    IQueryable<PackageItem> GetAll(bool ignoreFilters = false);

    ValueTask<PackageItem> AddAsync(PackageItem entity);

    ValueTask<PackageItem> UpdateAsync(PackageItem entity);

    ValueTask DeleteAsync(Guid id);

    ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<PackageItem>>> AddOrUpdate(IEnumerable<PackageItem> items);

    ValueTask DeleteAllAsync(IEnumerable<PackageItem> items);
}
