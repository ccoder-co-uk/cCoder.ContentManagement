using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Component = cCoder.Data.Models.CMS.Component;
using User = cCoder.Data.Models.Security.User;

namespace cCoder.ContentManagement.Services.Processings;

public interface IComponentRenderProcessingService
{
    string Render(int appId, string name, User user, string culture, string theme);

    string RenderComponent(Component component, ComponentRenderParams renderParams);
}
