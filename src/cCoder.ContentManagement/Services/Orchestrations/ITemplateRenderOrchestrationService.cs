// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal interface ITemplateRenderOrchestrationService
{
    TemplateRenderResult RenderTemplateRenderResult(
        int appId,
        string name,
        string culture,
        dynamic model);
}