// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal interface IUncachedPageRenderOrchestrationService
{
    ValueTask<HttpPageRenderOperation> RenderHttpPageRenderOperationAsync(
        HttpPageRenderOperation operation);
}