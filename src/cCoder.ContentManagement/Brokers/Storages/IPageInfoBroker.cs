// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IPageInfoBroker
{
    IQueryable<PageInfo> GetAllPageInfo();

    IQueryable<PageInfo> GetAllPageInfoIgnoringFilters();

    ValueTask<PageInfo> AddPageInfoAsync(PageInfo newPageInfo);

    ValueTask<PageInfo> UpdatePageInfoAsync(PageInfo updatedPageInfo);

    ValueTask<int> DeletePageInfoAsync(PageInfo deletedPageInfo);

    ValueTask DeleteAllPageInfoAsync(IEnumerable<PageInfo> deletedPageInfo);
}