// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal interface IComponentRenderOrchestrationService
{
    ComponentRenderResult RenderComponentRenderResult(
        int appId,
        string name,
        string culture,
        string theme);
}