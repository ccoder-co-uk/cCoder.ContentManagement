// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
namespace cCoder.ContentManagement.Services.Processings;

internal interface IPageRenderProcessingService
{
    PageRenderOperation RenderPageRenderOperation(
        PageRenderOperation operation);
}