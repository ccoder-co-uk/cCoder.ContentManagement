// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IPageInfoBroker
{
    IQueryable<PageInfo> GetAllPageInfos();

    IQueryable<PageInfo> GetAllPageInfosIgnoringFilters();

    int? GetOwningAppId(int pageId);

    ValueTask<PageInfo> AddPageInfoAsync(PageInfo newPageInfo);

    ValueTask<PageInfo> UpdatePageInfoAsync(PageInfo updatedPageInfo);

    ValueTask<int> DeletePageInfoAsync(PageInfo deletedPageInfo);

    ValueTask DeleteAllPageInfoAsync(IEnumerable<PageInfo> deletedPageInfo);
}