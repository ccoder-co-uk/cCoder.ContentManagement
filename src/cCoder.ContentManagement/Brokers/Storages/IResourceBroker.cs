using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IResourceBroker
{
    IQueryable<Resource> GetAllResources(bool ignoreFilters);

    ValueTask<Resource> AddResourceAsync(Resource entity);

    ValueTask<Resource> UpdateResourceAsync(Resource entity);

    ValueTask<int> DeleteResourceAsync(Resource entity);

    ValueTask DeleteAllResourcesAsync(IEnumerable<Resource> items);
}
