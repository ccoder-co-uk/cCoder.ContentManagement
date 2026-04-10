using PackageItem = cCoder.Data.Models.Packaging.PackageItem;
using Result = cCoder.ContentManagement.Models.Result<cCoder.Data.Models.Packaging.PackageItem>;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IPackageItemOrchestrationService
{
    PackageItem Get(Guid id);

    IQueryable<PackageItem> GetAll(bool ignoreFilters = false);

    ValueTask<PackageItem> AddAsync(PackageItem entity);

    ValueTask<PackageItem> UpdateAsync(PackageItem entity);

    ValueTask DeleteAsync(Guid id);

    ValueTask<IEnumerable<Result>> AddOrUpdate(IEnumerable<PackageItem> items);

    ValueTask DeleteAllAsync(IEnumerable<PackageItem> items);
}
