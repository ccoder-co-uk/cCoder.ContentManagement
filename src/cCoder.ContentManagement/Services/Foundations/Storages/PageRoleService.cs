// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PageRoleService(
    IPageRoleBroker pageRoleBroker) : IPageRoleService
{
    public bool PageRoleExists(PageRole pageRole) =>
        TryCatch<bool>(operation: () =>
    {
        ValidatePageRoleExistence(inputs: [pageRole]);

        return pageRoleBroker.GetAllPageRolesIgnoringFilters()
            .Any(predicate: existing =>
                existing.PageId == pageRole.PageId
                && existing.RoleId == pageRole.RoleId);
    });

    public PageRole ResolvePageRoleByIds(PageRole pageRole) =>
        TryCatch<PageRole>(operation: () =>
    {
        ValidatePageRoleAuthorization(inputs: [pageRole]);
        return pageRoleBroker.ResolvePageRoleByIds(pageRole: pageRole);
    });

    public PageRole ResolvePageRoleByNames(PageRole pageRole) =>
        TryCatch<PageRole>(operation: () =>
    {
        ValidatePageRoleAuthorization(inputs: [pageRole]);
        return pageRoleBroker.ResolvePageRoleByNames(pageRole: pageRole);
    });

    public ValueTask<PageRole> AddPageRoleForImportAsync(PageRole newPageRole) =>
        TryCatch<PageRole>(operation: () =>
    {
        ValidatePageRoleForImportOnAdd(inputs: [newPageRole]);
        ValidatePageRole(pageRole: newPageRole, parameterName: "pageRole");

        return pageRoleBroker.AddPageRoleAsync(newPageRole: newPageRole);
    }, isValueTask: true);

    public IQueryable<PageRole> GetAllPageRolesIgnoringFilters(bool ignoreFilters = true) =>
        TryCatch<IQueryable<PageRole>>(operation: () =>
    {
        ValidateAllPageRolesIgnoringFiltersOnGet(inputs: [ignoreFilters]);
        return pageRoleBroker.GetAllPageRolesIgnoringFilters();
    });

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

}