// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Exposures;

public interface ILayoutManager
{
    Layout GetLayout(int layoutId);

    IQueryable<Layout> GetAllLayout(bool ignoreFilters = false);

    ValueTask<Layout> AddLayoutAsync(Layout newLayout);

    ValueTask<Layout> UpdateLayoutAsync(Layout updatedLayout);

    ValueTask DeleteAsync(int layoutId);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<OperationResult<Layout>>> AddOrUpdateLayoutResult(IEnumerable<Layout> newLayout);

    ValueTask ImportLayoutsAsync(int appId, Layout[] items);

    ValueTask DeleteAllLayoutAsync(IEnumerable<Layout> deletedLayout);
}