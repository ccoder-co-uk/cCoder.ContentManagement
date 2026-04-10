using Package = cCoder.Data.Models.Packaging.Package;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IPackageOrchestrationService
{
    Package[] ExportPagackages(int appId, string[] packageNames);

    ValueTask ImportPackageAsync(int appId, Package package);

    Package Get(Guid id);

    IQueryable<Package> GetAll(bool ignoreFilters = false);

    ValueTask<Package> AddAsync(Package entity);

    ValueTask<Package> UpdateAsync(Package entity);

    ValueTask DeleteAsync(Guid id);

    ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<Package>>> AddOrUpdate(IEnumerable<Package> items);

    ValueTask DeleteAllAsync(IEnumerable<Package> items);
}
