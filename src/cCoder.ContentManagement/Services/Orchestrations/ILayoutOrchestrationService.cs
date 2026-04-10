using Layout = cCoder.Data.Models.CMS.Layout;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface ILayoutOrchestrationService
{
    Layout Get(int id);

    IQueryable<Layout> GetAll(bool ignoreFilters = false);

    ValueTask<Layout> AddAsync(Layout entity);

    ValueTask<Layout> UpdateAsync(Layout entity);

    ValueTask DeleteAsync(int id);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<Layout>>> AddOrUpdate(IEnumerable<Layout> items);

    ValueTask ImportLayoutsAsync(int appId, Layout[] items);

    ValueTask DeleteAllAsync(IEnumerable<Layout> items);
}
