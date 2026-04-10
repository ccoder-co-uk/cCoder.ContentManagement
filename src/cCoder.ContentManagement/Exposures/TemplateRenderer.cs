using cCoder.ContentManagement.Services.Coordinations;

namespace cCoder.ContentManagement.Exposures;

internal sealed class TemplateRenderer(ITemplateRenderCoordinationService renderCoordinationService) : ITemplateRenderer
{
    public string Render(int appId, string name, string culture, dynamic model) =>
        renderCoordinationService.Render(appId, name, culture, model);
}
