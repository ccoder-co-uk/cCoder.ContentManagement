// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IComponentRenderOrchestrationService
{
    string Render(int appId, string name, User user, string culture, string theme);
}