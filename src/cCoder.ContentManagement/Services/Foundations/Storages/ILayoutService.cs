// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal interface ILayoutService
{
    Layout GetLayout(int layoutId, bool ignoreFilters = false);

    IQueryable<Layout> GetAllLayout(bool ignoreFilters = false);

    ValueTask<Layout> AddLayoutAsync(Layout newLayout);

    ValueTask<Layout> UpdateLayoutAsync(Layout updatedLayout);

    ValueTask DeleteAsync(int layoutId);
}