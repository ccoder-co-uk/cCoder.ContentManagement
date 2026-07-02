using cCoder.Data.Models.Packaging;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

public interface IPackageItemProcessingService
{
    PackageItem Get(Guid id);

    IQueryable<PackageItem> GetAll(bool ignoreFilters = false);

    ValueTask<PackageItem> AddAsync(PackageItem entity);

    ValueTask<PackageItem> UpdateAsync(PackageItem entity);

    ValueTask DeleteAsync(Guid id);

    ValueTask<IEnumerable<Result<PackageItem>>> AddOrUpdate(IEnumerable<PackageItem> items);

    ValueTask DeleteAllAsync(IEnumerable<PackageItem> items);
}
