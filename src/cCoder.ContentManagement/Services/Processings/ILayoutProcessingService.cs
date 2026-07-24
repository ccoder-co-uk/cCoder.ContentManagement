// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

public interface ILayoutProcessingService
{
    Layout GetLayout(int layoutId);

    IQueryable<Layout> GetAllLayout(bool ignoreFilters = false);

    ValueTask<Layout> AddLayoutAsync(Layout newLayout);

    ValueTask<Layout> UpdateLayoutAsync(Layout updatedLayout);

    ValueTask DeleteAsync(int layoutId);

    ValueTask<IEnumerable<Result<Layout>>> AddOrUpdateLayoutResult(IEnumerable<Layout> newLayout);

    ValueTask DeleteAllLayoutAsync(IEnumerable<Layout> deletedLayout);
}