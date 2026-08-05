// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

public interface IPageManager
{
    IQueryable<Page> GetAll();

    Page Get(int pageId);

    Page GetRoot(int pageId);

    string GetMenu(int pageId, string culture);

    ValueTask<Page> AddAsync(Page newPage);

    ValueTask<Page> UpdateAsync(Page updatedPage);

    ValueTask DeleteAsync(int pageId);
}