// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Coordinations;

public interface IComponentRenderCoordinationService
{
    string Render(int appId, string name, string culture, string theme);
}