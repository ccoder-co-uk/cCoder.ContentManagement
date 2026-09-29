// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations.PageContexts;

internal interface IPageContextOrchestrationService
{
    ValueTask<HttpPageRenderContext> ResolvePageRenderContextAsync();
}