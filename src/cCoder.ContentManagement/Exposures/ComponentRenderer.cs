// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Orchestrations;

namespace cCoder.ContentManagement.Exposures;

internal sealed class ComponentRenderer(
    IComponentRenderOrchestrationService renderOrchestrationService)
        : IComponentRenderer
{
    public string Render(int appId, string name, string culture, string theme) =>
        renderOrchestrationService.RenderComponentRenderResult(
            appId: appId,
            name: name,
            culture: culture,
            theme: theme).Content;
}