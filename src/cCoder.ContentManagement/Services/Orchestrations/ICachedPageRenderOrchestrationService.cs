// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal interface ICachedPageRenderOrchestrationService
{
    ValueTask<HttpPageRenderOperation> RenderHttpPageRenderOperationAsync(
        HttpPageRenderOperation operation);
}