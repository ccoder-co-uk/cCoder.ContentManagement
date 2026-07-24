// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Processings;

public interface IPageRoleProcessingService
{
    IQueryable<PageRole> GetAllPageRole(bool ignoreFilters = false);

    ValueTask<PageRole> AddPageRoleAsync(PageRole newPageRole);

    ValueTask DeletePageRoleAsync(PageRole deletedPageRole);

    ValueTask<IEnumerable<OperationResult<PageRole>>> AddOrUpdatePageRoleResult(IEnumerable<PageRole> newPageRole);

    ValueTask ImportPageRoleInfosAsync(int appId, PageRoleInfo[] items);

    ValueTask DeleteAllPageRoleAsync(IEnumerable<PageRole> deletedPageRole);
}