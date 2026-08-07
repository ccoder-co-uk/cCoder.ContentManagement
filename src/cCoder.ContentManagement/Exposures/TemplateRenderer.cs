// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Orchestrations;

namespace cCoder.ContentManagement.Exposures;

internal sealed class TemplateRenderer(
    ITemplateRenderOrchestrationService renderOrchestrationService)
        : ITemplateRenderer
{
    public string Render(int appId, string name, string culture, dynamic model) =>
        renderOrchestrationService.RenderTemplateRenderResult(
            appId: appId,
            name: name,
            culture: culture,
            model: model).Content;
}