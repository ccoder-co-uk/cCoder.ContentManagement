namespace cCoder.ContentManagement.Exposures;

public interface IComponentRenderer
{
    string Render(int appId, string name, string culture, string theme);
}
