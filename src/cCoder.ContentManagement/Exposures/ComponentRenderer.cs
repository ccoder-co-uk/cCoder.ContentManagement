// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Coordinations;

namespace cCoder.ContentManagement.Exposures;

internal sealed class ComponentRenderer(IComponentRenderCoordinationService renderCoordinationService) : IComponentRenderer
{
    public string Render(int appId, string name, string culture, string theme) =>
        renderCoordinationService.Render(appId: appId, name: name, culture: culture, theme: theme);
}