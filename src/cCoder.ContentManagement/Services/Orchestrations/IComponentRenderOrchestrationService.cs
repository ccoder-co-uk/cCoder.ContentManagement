// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal interface IComponentRenderOrchestrationService
{
    string Render(int appId, string name, string culture, string theme);

    string RenderUser(int appId, string name, User user, string culture, string theme);
}