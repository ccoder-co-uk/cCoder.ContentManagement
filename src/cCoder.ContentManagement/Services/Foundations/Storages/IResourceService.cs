using Resource = cCoder.Data.Models.CMS.Resource;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

public interface IResourceService
{
    Resource Get(int id, bool ignoreFilters = false);

    IQueryable<Resource> GetAll(bool ignoreFilters = false);

    ValueTask<Resource> AddAsync(Resource resource);

    ValueTask<Resource> UpdateAsync(Resource resource);

    ValueTask DeleteAsync(int id);
}
