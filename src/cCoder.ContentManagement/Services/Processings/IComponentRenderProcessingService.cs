// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Processings;

public interface IComponentRenderProcessingService
{
    string RenderUser(int appId, string name, User user, string culture, string theme);

    string RenderComponentComponentRenderParams(Component component, ComponentRenderParams renderParams);
}