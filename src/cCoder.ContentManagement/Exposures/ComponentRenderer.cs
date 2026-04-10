using cCoder.ContentManagement.Services.Coordinations;

namespace cCoder.ContentManagement.Exposures;

internal sealed class ComponentRenderer(IComponentRenderCoordinationService renderCoordinationService) : IComponentRenderer
{
    public string Render(int appId, string name, string culture, string theme) =>
        renderCoordinationService.Render(appId, name, culture, theme);
}
