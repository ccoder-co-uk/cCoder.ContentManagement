namespace cCoder.ContentManagement.Services.Coordinations;

public interface ITemplateRenderCoordinationService
{
    string Render(int appId, string name, string culture, dynamic model);
}
