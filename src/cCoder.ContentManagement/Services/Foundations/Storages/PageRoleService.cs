// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

using cCoder.ContentManagement.Exposures;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PageRoleService(
    IPageRoleBroker pageRoleBroker,
    IPageBroker pageBroker,
    IRoleBroker roleBroker,
    IAuthorizationManager authorizationManager) : IPageRoleService
{
    public bool UserCanAddPageRole(PageRole pageRole) =>
        TryCatch<bool>(operation: () =>
    {
        ValidatePageRoleAuthorization(inputs: [pageRole]);
        Page page = GetPage(pageId: pageRole.PageId);
        bool roleExists = roleBroker.GetAllRolesIgnoringFilters()
            .Any(predicate: role => role.Id == pageRole.RoleId);

        return roleExists && page != null && UserCanPage(
            page: page,
            privilege: "pagerole_create");
    });

    public bool UserCanDeletePageRole(PageRole pageRole) =>
        TryCatch<bool>(operation: () =>
    {
        ValidatePageRoleAuthorization(inputs: [pageRole]);
        Page page = GetPage(pageId: pageRole.PageId);

        return page != null && UserCanPage(
            page: page,
            privilege: "pagerole_delete");
    });

    public bool PageRoleExists(PageRole pageRole) =>
        TryCatch<bool>(operation: () =>
    {
        ValidatePageRoleExistence(inputs: [pageRole]);
        Page page = GetPage(pageId: pageRole.PageId);

        return (page?.Roles ?? [])
            .Any(predicate: existing => existing.RoleId == pageRole.RoleId);
    });

    public PageRole ResolvePageRole(int appId, string path, string roleName) =>
        TryCatch<PageRole>(operation: () =>
    {
        ValidatePageRoleOnResolve(inputs: [appId, path, roleName]);

        Guid roleId = roleBroker.GetAllRolesIgnoringFilters()
            .Where(predicate: role => role.AppId == appId && role.Name == roleName)
            .Select(selector: role => role.Id)
            .FirstOrDefault();

        int pageId = pageBroker.GetAllPagesIgnoringFilters()
            .Where(predicate: page => page.AppId == appId && page.Path == path)
            .Select(selector: page => page.Id)
            .FirstOrDefault();

        return new PageRole
        {
            PageId = pageId,
            RoleId = roleId
        };
    });

    public ValueTask<PageRole> AddPageRoleForImportAsync(PageRole newPageRole) =>
        TryCatch<PageRole>(operation: () =>
            pageRoleBroker.AddPageRoleAsync(newPageRole: newPageRole),
            isValueTask: true);

    public IQueryable<PageRole> GetAllPageRolesIgnoringFilters() =>
        pageRoleBroker.GetAllPageRolesIgnoringFilters();

    public ValueTask DeleteAllPageRolesAsync(IEnumerable<PageRole> deletedPageRole) =>
        TryCatch(operation: async () =>
    {
        ValidateAllPageRolesOnDelete(inputs: [deletedPageRole]);
        await pageRoleBroker.DeleteAllPageRolesAsync(
            deletedPageRole: deletedPageRole);
    }, isValueTask: true);


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

    private Page GetPage(int pageId) =>
        pageBroker.GetAllPagesIgnoringFilters()
            .FirstOrDefault(predicate: page => page.Id == pageId);

    private bool UserCanPage(Page page, string privilege) =>
        authorizationManager.UserCanPageAuthorization(
            pageAuthorization: new PageAuthorization
            {
                Page = page,
                User = authorizationManager.GetCurrentUser(),
                Privilege = privilege
            });

    private int? GetAppId(int pageId) =>
        pageBroker.GetAllPagesIgnoringFilters()
        .Where(predicate: page => page.Id == pageId)
        .Select(selector: page => (int?)page.AppId)
        .FirstOrDefault();

}