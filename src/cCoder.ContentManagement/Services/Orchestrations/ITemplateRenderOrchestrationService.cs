// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal interface ITemplateRenderOrchestrationService
{
    string Render(int appId, string name, string culture, dynamic model);

    string RenderUser(int appId, string name, string culture, dynamic model, User user);
}