// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PageRenderDataService(
    IPageRenderDataBroker broker) : IPageRenderDataService
{
    public ValueTask<PageRenderData> GetPageRenderDataAsync(int pageId) =>
        TryCatch<PageRenderData>(operation: async () =>
        {
            ValidatePageRenderDataOnGet(inputs: [pageId]);
            ValidatePageId(pageId: pageId, parameterName: "pageId");

            return await broker.GetPageRenderDataAsync(pageId: pageId);
        }, isValueTask: true);
}