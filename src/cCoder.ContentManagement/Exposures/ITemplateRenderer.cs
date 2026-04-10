namespace cCoder.ContentManagement.Exposures;

public interface ITemplateRenderer
{
    string Render(int appId, string name, string culture, dynamic model);
}
