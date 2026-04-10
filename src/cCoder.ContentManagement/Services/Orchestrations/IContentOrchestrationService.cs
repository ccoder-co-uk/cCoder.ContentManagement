using Content = cCoder.Data.Models.CMS.Content;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IContentOrchestrationService
{
    Content Get(int id);

    IQueryable<Content> GetAll(bool ignoreFilters = false);

    ValueTask<Content> AddAsync(Content entity);

    ValueTask<Content> UpdateAsync(Content entity);

    ValueTask DeleteAsync(int id);

    ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<Content>>> AddOrUpdate(IEnumerable<Content> items);

    ValueTask DeleteAllAsync(IEnumerable<Content> items);
}
