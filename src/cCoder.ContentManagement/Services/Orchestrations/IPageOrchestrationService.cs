// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal interface IPageOrchestrationService
{
    Page GetPage(int pageId);

    ValueTask<Page> GetPageByIdForRenderAsync(int pageId);

    IQueryable<Page> GetAllPage(bool ignoreFilters = false);

    ValueTask<Page> AddPageAsync(Page newPage);

    ValueTask<Page> UpdatePageAsync(Page updatedPage);

    ValueTask DeleteAsync(int pageId);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<OperationResult<Page>>> AddOrUpdatePageResult(IEnumerable<Page> newPage);

    ValueTask ImportPagesAsync(int appId, Page[] items);

    ValueTask DeleteAllPageAsync(IEnumerable<Page> deletedPage);

    ValueTask RecomputeAllForAppAsync(int appId);

    Page GetRootPage(int pageId);

    IEnumerable<Page> GetChildrenPage(int pageId);

    string MenuFor(int pageId, string culture);
}