using PageRole = cCoder.Data.Models.Security.PageRole;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;

namespace cCoder.ContentManagement.Services.Processings;

public interface IPageRoleProcessingService
{
    IQueryable<PageRole> GetAll(bool ignoreFilters = false);

    ValueTask<PageRole> AddAsync(PageRole entity);

    ValueTask DeleteAsync(PageRole entity);

    ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<PageRole>>> AddOrUpdate(IEnumerable<PageRole> items);

    ValueTask ImportPageRolesAsync(int appId, PageRoleInfo[] items);

    ValueTask DeleteAllAsync(IEnumerable<PageRole> items);
}
