// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IPageRoleBroker
{
    IQueryable<PageRole> GetAllPageRoles();

    IQueryable<PageRole> GetAllPageRolesIgnoringFilters();

    PageRole ResolvePageRoleByIds(PageRole pageRole);

    PageRole ResolvePageRoleByNames(PageRole pageRole);

    ValueTask<PageRole> AddPageRoleAsync(PageRole newPageRole);

    ValueTask<int> DeletePageRoleAsync(PageRole deletedPageRole);

    ValueTask DeleteAllPageRolesAsync(IEnumerable<PageRole> deletedPageRole);
}