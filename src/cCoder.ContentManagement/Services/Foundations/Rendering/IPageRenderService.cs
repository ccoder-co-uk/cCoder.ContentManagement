// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Rendering;

namespace cCoder.ContentManagement.Services.Foundations.Rendering;

internal interface IPageRenderService
{
    PageRenderFoundationOperation SerializePageRenderFoundationOperation(PageRenderFoundationOperation pageRenderFoundationOperation);
    PageRenderFoundationOperation RenderPageRenderFoundationOperation(PageRenderFoundationOperation pageRenderFoundationOperation);
}