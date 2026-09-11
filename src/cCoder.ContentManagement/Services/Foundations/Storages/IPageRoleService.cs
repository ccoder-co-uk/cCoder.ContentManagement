// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal interface IPageRoleService
{
    IQueryable<PageRole> GetAllPageRole(bool ignoreFilters = false);

    ValueTask<PageRole> AddPageRoleAsync(PageRole newPageRole);

    ValueTask DeletePageRoleAsync(PageRole deletedPageRole);

    bool UserCanAddPageRole(PageRole pageRole);

    bool UserCanDeletePageRole(PageRole pageRole);

    bool PageRoleExists(PageRole pageRole);

    PageRole ResolvePageRole(int appId, string path, string roleName);

    ValueTask<PageRole> AddPageRoleForImportAsync(PageRole newPageRole);

    IQueryable<PageRole> GetAllPageRolesIgnoringFilters();

    ValueTask DeleteAllPageRolesAsync(IEnumerable<PageRole> deletedPageRole);
}