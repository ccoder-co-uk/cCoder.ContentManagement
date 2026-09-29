// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IPageInfoProcessingService
{
    PageInfo GetPageInfo(int pageInfoId);

    IQueryable<PageInfo> GetAllPageInfo(bool ignoreFilters = false);

    int? GetOwningAppId(int pageId);

    ValueTask<PageInfo> AddPageInfoAsync(PageInfo newPageInfo);

    ValueTask<PageInfo> UpdatePageInfoAsync(PageInfo updatedPageInfo);

    ValueTask DeleteAsync(int pageInfoId);

    ValueTask<IEnumerable<OperationResult<PageInfo>>> AddOrUpdatePageInfoResult(IEnumerable<PageInfo> newPageInfo);

    ValueTask DeleteAllPageInfoAsync(IEnumerable<PageInfo> deletedPageInfo);
}