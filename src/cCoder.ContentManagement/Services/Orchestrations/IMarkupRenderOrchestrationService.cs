// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal interface IMarkupRenderOrchestrationService
{
    ValueTask RenderHttpPageRenderOperationAsync(
        HttpPageRenderOperation httpPageRenderOperation);
}