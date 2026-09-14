// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models.Rendering;
using cCoder.ContentManagement.Rendering.Brokers;

namespace cCoder.ContentManagement.Services.Foundations.Rendering;

internal sealed partial class PageRenderService(
    IRenderBroker renderBroker,
    IJsonBroker jsonBroker)
        : IPageRenderService
{
    public PageRenderFoundationOperation SerializePageRenderFoundationOperation(
        PageRenderFoundationOperation pageRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidatePageRenderFoundationOperation(inputs: [pageRenderFoundationOperation]);
        pageRenderFoundationOperation.Json = jsonBroker.Serialize(value: pageRenderFoundationOperation.Value);
        return pageRenderFoundationOperation;
    });

    public PageRenderFoundationOperation RenderPageRenderFoundationOperation(
        PageRenderFoundationOperation pageRenderFoundationOperation) =>
        TryCatch(operation: () =>
    {
        ValidatePageRenderFoundationOperation(inputs: [pageRenderFoundationOperation]);
        pageRenderFoundationOperation.RenderSession = renderBroker.RenderRenderSession(renderSession: pageRenderFoundationOperation.RenderSession);
        return pageRenderFoundationOperation;
    });
}