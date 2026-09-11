// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

internal sealed class PageManager(
    IPageOrchestrationService pageOrchestrationService)
        : IPageManager
{
    public IQueryable<Page> GetAll() =>
        pageOrchestrationService.GetAllPage();

    public Page Get(int pageId) =>
        pageOrchestrationService.GetPage(pageId: pageId);

    public Page GetRoot(int pageId) =>
        pageOrchestrationService.GetRootPage(pageId: pageId);

    public string GetMenu(int pageId, string culture) =>
        pageOrchestrationService.MenuFor(
            pageId: pageId,
            culture: culture);

    public ValueTask<Page> AddAsync(Page newPage) =>
        pageOrchestrationService.AddPageAsync(newPage: newPage);

    public ValueTask<Page> UpdateAsync(Page updatedPage) =>
        pageOrchestrationService.UpdatePageAsync(updatedPage: updatedPage);

    public ValueTask DeleteAsync(int pageId) =>
        pageOrchestrationService.DeleteAsync(pageId: pageId);
}