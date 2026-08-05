// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal interface IPageService
{
    Page GetPage(int pageId, bool ignoreFilters = false);

    ValueTask<Page> GetPageForRenderAsync(int pageId);

    IQueryable<Page> GetAllPage(bool ignoreFilters = false);

    ValueTask<Page> AddPageAsync(Page newPage);

    ValueTask<Page> UpdatePageAsync(Page updatedPage);

    ValueTask DeleteAsync(int pageId);
}