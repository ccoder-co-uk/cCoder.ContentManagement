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

    bool PageRoleExists(PageRole pageRole);

    PageRole ResolvePageRoleByIds(PageRole pageRole);

    PageRole ResolvePageRoleByNames(PageRole pageRole);

    ValueTask<PageRole> AddPageRoleForImportAsync(PageRole newPageRole);

    IQueryable<PageRole> GetAllPageRolesIgnoringFilters(bool ignoreFilters = true);

    ValueTask DeleteAllPageRolesAsync(IEnumerable<PageRole> deletedPageRole);
}