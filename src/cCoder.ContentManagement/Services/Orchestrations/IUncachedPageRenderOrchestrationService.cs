// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal interface IUncachedPageRenderOrchestrationService
{
    ValueTask PrepareHttpPageRenderOperationAsync(
        HttpPageRenderOperation httpPageRenderOperation);

    ValueTask CompleteHttpPageRenderOperationAsync(
        HttpPageRenderOperation httpPageRenderOperation);
}