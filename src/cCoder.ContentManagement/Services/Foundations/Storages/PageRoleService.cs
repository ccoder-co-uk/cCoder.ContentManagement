using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using PageRole = cCoder.Data.Models.Security.PageRole;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PageRoleService(
    IPageRoleBroker pageRoleBroker,
    IPageBroker pageBroker,
    IAuthorizationBroker authorizationBroker) : IPageRoleService
{
    public IQueryable<PageRole> GetAll(bool ignoreFilters = false)
    {
        return pageRoleBroker.GetAllPageRoles(ignoreFilters);
    }

    public async ValueTask<PageRole> AddAsync(PageRole pageRole)
    {
        ValidatePageRole(pageRole, "pageRole");
        authorizationBroker.Authorize(GetAppId(pageRole.PageId), "PageRole_create");
        PageRole result = await pageRoleBroker.AddPageRoleAsync(CreateStoragePageRole(pageRole));
        pageRole.PageId = result.PageId;
        pageRole.RoleId = result.RoleId;
        return pageRole;
    }

    public async ValueTask DeleteAsync(PageRole pageRole)
    {
        ValidatePageRole(pageRole, "pageRole");
        authorizationBroker.Authorize(GetAppId(pageRole.PageId), "PageRole_delete");
        await pageRoleBroker.DeletePageRoleAsync(CreateStoragePageRole(pageRole));
    }

    private static PageRole CreateStoragePageRole(PageRole pageRole)
    {
        if (pageRole == null)
        {
            return null;
        }

        return new PageRole
        {
            PageId = pageRole.PageId,
            RoleId = pageRole.RoleId
        };
    }

    private int? GetAppId(int pageId)
    {
        return pageBroker.GetAllPages(ignoreFilters: true)
            .Where(page => page.Id == pageId)
            .Select(page => (int?)page.AppId)
            .FirstOrDefault();
    }
}
