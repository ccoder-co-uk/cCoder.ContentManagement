using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using PageRole = cCoder.Data.Models.Security.PageRole;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IPageRoleOrchestrationService
{
    IQueryable<PageRole> GetAll(bool ignoreFilters = false);

    ValueTask<PageRole> AddAsync(PageRole entity);

    ValueTask DeleteAsync(PageRole entity);

    ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<PageRole>>> AddOrUpdate(IEnumerable<PageRole> items);

    ValueTask ImportPageRolesAsync(int appId, PageRoleInfo[] items);

    ValueTask DeleteAllAsync(IEnumerable<PageRole> items);
}
