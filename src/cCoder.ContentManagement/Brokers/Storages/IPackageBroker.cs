using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IPackageBroker
{
    IQueryable<Package> GetAllPackages(bool ignoreFilters);

    ValueTask<Package> AddPackageAsync(Package entity);

    ValueTask<Package> UpdatePackageAsync(Package entity);

    ValueTask<int> DeletePackageAsync(Package entity);

    ValueTask DeleteAllPackagesAsync(IEnumerable<Package> items);
}
