// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IPageBroker
{
    IQueryable<Page> GetAllPages();

    IQueryable<Page> GetAllPagesIgnoringFilters();

    bool LayoutExistsForApp(int appId, string layoutName);

    ValueTask<Page> AddPageAsync(Page newPage);

    ValueTask<Page> UpdatePageAsync(Page updatedPage);

    ValueTask<int> DeletePageAsync(Page deletedPage);

    ValueTask DeleteAllPagesAsync(IEnumerable<Page> deletedPage);
}