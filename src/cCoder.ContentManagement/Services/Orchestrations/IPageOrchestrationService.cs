// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IPageOrchestrationService
{
    Page GetPage(int pageId);

    IQueryable<Page> GetAllPages(bool ignoreFilters = false);

    ValueTask<Page> AddPageAsync(Page newPage);

    ValueTask<Page> UpdatePageAsync(Page updatedPage);

    ValueTask DeleteAsync(int pageId);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<OperationResult<Page>>> AddOrUpdatePageResult(IEnumerable<Page> newPage);

    ValueTask<Page[]> ImportPagesAsync(int appId, Page[] items);

    ValueTask DeleteAllPageAsync(IEnumerable<Page> deletedPage);

    ValueTask RecomputeAllForAppAsync(int appId);

    Page GetRootPage(int pageId);

    IEnumerable<Page> GetChildrenPages(int pageId);

    string MenuFor(int pageId, string culture);
}