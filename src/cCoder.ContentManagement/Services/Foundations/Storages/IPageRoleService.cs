// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal interface IPageRoleService
{
    IQueryable<PageRole> GetAllPageRole(bool ignoreFilters = false);

    ValueTask<PageRole> AddPageRoleAsync(PageRole newPageRole);

    ValueTask DeletePageRoleAsync(PageRole deletedPageRole);
}