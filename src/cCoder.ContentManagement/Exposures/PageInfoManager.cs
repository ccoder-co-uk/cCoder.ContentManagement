// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

internal sealed class PageInfoManager(IPageInfoOrchestrationService service) : IPageInfoManager
{
    public PageInfo GetPageInfo(int pageInfoId) =>
        service.GetPageInfo(pageInfoId: pageInfoId);

    public IQueryable<PageInfo> GetAllPageInfos(bool ignoreFilters = false) =>
        service.GetAllPageInfos(ignoreFilters: ignoreFilters);

    public ValueTask<PageInfo> AddPageInfoAsync(PageInfo newPageInfo) =>
        service.AddPageInfoAsync(newPageInfo: newPageInfo);

    public ValueTask<PageInfo> UpdatePageInfoAsync(PageInfo updatedPageInfo) =>
        service.UpdatePageInfoAsync(updatedPageInfo: updatedPageInfo);

    public ValueTask DeleteAsync(int pageInfoId) =>
        service.DeleteAsync(pageInfoId: pageInfoId);

    public ValueTask<IEnumerable<OperationResult<PageInfo>>> AddOrUpdatePageInfoResult(
        IEnumerable<PageInfo> newPageInfo) =>
        service.AddOrUpdatePageInfoResult(newPageInfo: newPageInfo);

    public ValueTask DeleteAllPageInfoAsync(IEnumerable<PageInfo> deletedPageInfo) =>
        service.DeleteAllPageInfoAsync(deletedPageInfo: deletedPageInfo);
}