// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

public interface IPageInfoManager
{
    PageInfo GetPageInfo(int pageInfoId);

    IQueryable<PageInfo> GetAllPageInfos(bool ignoreFilters = false);

    ValueTask<PageInfo> AddPageInfoAsync(PageInfo newPageInfo);

    ValueTask<PageInfo> UpdatePageInfoAsync(PageInfo updatedPageInfo);

    ValueTask DeleteAsync(int pageInfoId);

    ValueTask<IEnumerable<OperationResult<PageInfo>>> AddOrUpdatePageInfoResult(IEnumerable<PageInfo> newPageInfo);

    ValueTask DeleteAllPageInfoAsync(IEnumerable<PageInfo> deletedPageInfo);
}