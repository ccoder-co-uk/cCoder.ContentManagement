// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Processings;

public interface IComponentRenderProcessingService
{
    string Render(int appId, string name, User user, string culture, string theme);

    string RenderComponent(Component component, ComponentRenderParams renderParams);
}