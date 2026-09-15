// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
namespace cCoder.ContentManagement.Services.Processings;

internal interface IPageRenderProcessingService
{
    string ComputeFingerprint(object value);

    string SerializeRuntimeValue(object value);
    PageRenderOperation RenderPageRenderOperation(
        PageRenderOperation operation);

}