// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

public interface IPageInfoService
{
    PageInfo GetPageInfo(int pageInfoId, bool ignoreFilters = false);

    IQueryable<PageInfo> GetAllPageInfo(bool ignoreFilters = false);

    ValueTask<PageInfo> AddPageInfoAsync(PageInfo newPageInfo);

    ValueTask<PageInfo> UpdatePageInfoAsync(PageInfo updatedPageInfo);

    ValueTask DeleteAsync(int pageInfoId);
}