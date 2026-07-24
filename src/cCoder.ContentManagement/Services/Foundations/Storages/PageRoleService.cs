// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PageRoleService(
    IPageRoleBroker pageRoleBroker,
    IPageBroker pageBroker,
    IAuthorizationBroker authorizationBroker) : IPageRoleService
{
    public IQueryable<PageRole> GetAll(bool ignoreFilters = false) =>
        pageRoleBroker.GetAllPageRoles(ignoreFilters: ignoreFilters);

    public async ValueTask<PageRole> AddAsync(PageRole pageRole)
    {
        ValidatePageRole(pageRole: pageRole, parameterName: "pageRole");
        authorizationBroker.Authorize(appId: GetAppId(pageId: pageRole.PageId), privilege: "PageRole_create");
        PageRole result = await pageRoleBroker.AddPageRoleAsync(entity: CreateStoragePageRole(pageRole: pageRole));
        pageRole.PageId = result.PageId;
        pageRole.RoleId = result.RoleId;
        return pageRole;
    }

    public async ValueTask DeleteAsync(PageRole pageRole)
    {
        ValidatePageRole(pageRole: pageRole, parameterName: "pageRole");
        authorizationBroker.Authorize(appId: GetAppId(pageId: pageRole.PageId), privilege: "PageRole_delete");
        await pageRoleBroker.DeletePageRoleAsync(entity: CreateStoragePageRole(pageRole: pageRole));
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
            .Where(predicate: page => page.Id == pageId)
            .Select(selector: page => (int?)page.AppId)
            .FirstOrDefault();
    }
}