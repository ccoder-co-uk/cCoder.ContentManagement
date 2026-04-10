using User = cCoder.Data.Models.Security.User;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IComponentRenderOrchestrationService
{
    string Render(int appId, string name, User user, string culture, string theme);
}
