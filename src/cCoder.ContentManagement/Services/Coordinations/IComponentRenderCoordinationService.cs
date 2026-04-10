namespace cCoder.ContentManagement.Services.Coordinations;

public interface IComponentRenderCoordinationService
{
    string Render(int appId, string name, string culture, string theme);
}
