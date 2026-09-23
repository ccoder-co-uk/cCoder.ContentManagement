// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class PageRenderDataProcessingService(
    IPageRenderDataService service) : IPageRenderDataProcessingService
{
    public ValueTask<Page> GetPageForRenderAsync(int pageId) =>
        TryCatch<Page>(operation: async () =>
        {
            ValidatePageForRenderOnGet(inputs: [pageId]);
            ValidatePageId(pageId: pageId, parameterName: "pageId");

            PageRenderData renderData =
                await service.GetPageRenderDataAsync(pageId: pageId);

            Page page = renderData.Page;

            if (page?.App is null)
            {
                return page;
            }

            page.App.Layouts = renderData.Layouts;
            page.App.Templates = renderData.Templates;
            page.App.Resources = renderData.Resources;
            page.App.Components = renderData.Components;
            page.App.Scripts = renderData.Scripts;
            page.App.Pages = renderData.Pages;

            return page;
        }, isValueTask: true);
}