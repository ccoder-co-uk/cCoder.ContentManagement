// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal interface IPageRoleService
{
    IQueryable<PageRole> GetAllPageRoles(bool ignoreFilters = false);

    ValueTask<PageRole> AddPageRoleAsync(PageRole newPageRole);

    ValueTask DeletePageRoleAsync(PageRole deletedPageRole);

    bool PageRoleExists(PageRole pageRole);

    PageRole ResolvePageRoleByIds(PageRole pageRole);

    PageRole ResolvePageRoleByNames(PageRole pageRole);

    ValueTask<PageRole> AddPageRoleForImportAsync(PageRole newPageRole);

    IQueryable<PageRole> GetAllPageRolesIgnoringFilters(bool ignoreFilters = true);

    ValueTask DeleteAllPageRolesAsync(IEnumerable<PageRole> deletedPageRole);
}