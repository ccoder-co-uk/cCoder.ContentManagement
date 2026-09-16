// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Rendering;

namespace cCoder.ContentManagement.Services.Foundations.Rendering;

internal interface IPageRenderService
{
    PageRenderFoundationOperation ComputeFingerprintPageRenderFoundationOperation(
        PageRenderFoundationOperation pageRenderFoundationOperation);
    PageRenderFoundationOperation SerializePageRenderFoundationOperation(
        PageRenderFoundationOperation pageRenderFoundationOperation);
}