// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
namespace cCoder.ContentManagement.Services.Processings;

internal interface IPageRenderProcessingService
{
    string ComputeFingerprint(string value);
    PageRenderOperation RenderPageRenderOperation(
        PageRenderOperation operation);

    string SerializeRuntimeValue(object value);
}