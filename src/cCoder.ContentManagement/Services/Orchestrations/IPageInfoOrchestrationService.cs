using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IPageInfoOrchestrationService
{
    PageInfo Get(int id);

    IQueryable<PageInfo> GetAll(bool ignoreFilters = false);

    ValueTask<PageInfo> AddAsync(PageInfo entity);

    ValueTask<PageInfo> UpdateAsync(PageInfo entity);

    ValueTask DeleteAsync(int id);

    ValueTask<IEnumerable<Result<PageInfo>>> AddOrUpdate(IEnumerable<PageInfo> items);

    ValueTask DeleteAllAsync(IEnumerable<PageInfo> items);
}
