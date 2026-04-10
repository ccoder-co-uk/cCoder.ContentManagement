using Page = cCoder.Data.Models.CMS.Page;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IPageOrchestrationService
{
    Page Get(int id);

    IQueryable<Page> GetAll(bool ignoreFilters = false);

    ValueTask<Page> AddAsync(Page entity);

    ValueTask<Page> UpdateAsync(Page entity);

    ValueTask DeleteAsync(int id);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<Page>>> AddOrUpdate(IEnumerable<Page> items);

    ValueTask ImportPagesAsync(int appId, Page[] items);

    ValueTask DeleteAllAsync(IEnumerable<Page> items);

    ValueTask RecomputeAllForAppAsync(int appId);

    Page GetRoot(int id);

    IEnumerable<Page> GetChildren(int id);

    string MenuFor(int id, string culture);
}
