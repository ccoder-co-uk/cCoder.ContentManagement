// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal interface IPageService
{
    Page GetPage(int pageId, bool ignoreFilters = false);

    IQueryable<Page> GetAllPages(bool ignoreFilters = false);

    bool LayoutExistsForApp(int appId, string layoutName);

    ValueTask<Page> AddPageAsync(Page newPage);

    ValueTask<Page> UpdatePageAsync(Page updatedPage);

    ValueTask DeleteAsync(int pageId);

}