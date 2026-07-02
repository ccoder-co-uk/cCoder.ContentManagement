using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IResourceOrchestrationService
{
    Resource Get(int id);

    IQueryable<Resource> GetAll(bool ignoreFilters = false);

    ValueTask<Resource> AddAsync(Resource entity);

    ValueTask<Resource> UpdateAsync(Resource entity);

    ValueTask DeleteAsync(int id);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<Result<Resource>>> AddOrUpdate(IEnumerable<Resource> items);

    ValueTask ImportResourcesAsync(int appId, Resource[] items);

    ValueTask DeleteAllAsync(IEnumerable<Resource> items);
}
