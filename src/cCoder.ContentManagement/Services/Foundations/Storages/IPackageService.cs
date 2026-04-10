using Package = cCoder.Data.Models.Packaging.Package;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

public interface IPackageService
{
    Package Get(Guid id, bool ignoreFilters = false);

    IQueryable<Package> GetAll(bool ignoreFilters = false);

    ValueTask<Package> AddAsync(Package package);

    ValueTask<Package> UpdateAsync(Package package);

    ValueTask DeleteAsync(Guid id);
}
