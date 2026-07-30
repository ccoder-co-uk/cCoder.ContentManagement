// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IPageInfoProcessingService
{
    PageInfo GetPageInfo(int pageInfoId);

    IQueryable<PageInfo> GetAllPageInfo(bool ignoreFilters = false);

    ValueTask<PageInfo> AddPageInfoAsync(PageInfo newPageInfo);

    ValueTask<PageInfo> UpdatePageInfoAsync(PageInfo updatedPageInfo);

    ValueTask DeleteAsync(int pageInfoId);

    ValueTask<IEnumerable<OperationResult<PageInfo>>> AddOrUpdatePageInfoResult(IEnumerable<PageInfo> newPageInfo);

    ValueTask DeleteAllPageInfoAsync(IEnumerable<PageInfo> deletedPageInfo);
}