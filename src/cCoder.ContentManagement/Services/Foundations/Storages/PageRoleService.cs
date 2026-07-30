// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.Security;

using cCoder.ContentManagement.Exposures;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PageRoleService(
    IPageRoleBroker pageRoleBroker,
    IPageBroker pageBroker,
    IAuthorizationManager authorizationManager) : IPageRoleService
{
    public IQueryable<PageRole> GetAllPageRole(bool ignoreFilters = false) =>
        TryCatch<IQueryable<PageRole>>(operation: () =>
    {
        ValidateAllPageRoleOnGet(inputs: [ignoreFilters]);

        return ignoreFilters
            ? pageRoleBroker.GetAllPageRolesIgnoringFilters()
            : pageRoleBroker.GetAllPageRoles();
    });

    public ValueTask<PageRole> AddPageRoleAsync(PageRole newPageRole) =>
        TryCatch<PageRole>(operation: async () =>
    {
        ValidatePageRoleOnAdd(inputs: [newPageRole]);
        ValidatePageRole(pageRole: newPageRole, parameterName: "pageRole");
        authorizationManager.Authorize(appId: GetAppId(pageId: newPageRole.PageId), privilege: "PageRole_create");
        PageRole result = await pageRoleBroker.AddPageRoleAsync(newPageRole: CreateStoragePageRole(newPageRole: newPageRole));
        newPageRole.PageId = result.PageId;
        newPageRole.RoleId = result.RoleId;
        return newPageRole;

    }, isValueTask: true);

    public ValueTask DeletePageRoleAsync(PageRole deletedPageRole) =>
        TryCatch(operation: async () =>
    {
        ValidatePageRoleOnDelete(inputs: [deletedPageRole]);
        ValidatePageRole(pageRole: deletedPageRole, parameterName: "pageRole");
        authorizationManager.Authorize(appId: GetAppId(pageId: deletedPageRole.PageId), privilege: "PageRole_delete");
        await pageRoleBroker.DeletePageRoleAsync(deletedPageRole: CreateStoragePageRole(newPageRole: deletedPageRole));

    }, isValueTask: true);

    private static PageRole CreateStoragePageRole(PageRole newPageRole)
    {
        if (newPageRole == null)
        {
            return null;
        }

        return new PageRole
        {
            PageId = newPageRole.PageId,
            RoleId = newPageRole.RoleId
        };
    }

    private int? GetAppId(int pageId) =>
        pageBroker.GetAllPagesIgnoringFilters()
        .Where(predicate: page => page.Id == pageId)
        .Select(selector: page => (int?)page.AppId)
        .FirstOrDefault();

}