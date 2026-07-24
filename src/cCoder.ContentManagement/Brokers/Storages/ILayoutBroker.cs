// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface ILayoutBroker
{
    IQueryable<Layout> GetAllLayouts(bool ignoreFilters);

    ValueTask<Layout> AddLayoutAsync(Layout newLayout);

    ValueTask<Layout> UpdateLayoutAsync(Layout updatedLayout);

    ValueTask<int> DeleteLayoutAsync(Layout deletedLayout);

    ValueTask DeleteAllLayoutsAsync(IEnumerable<Layout> deletedLayout);
}