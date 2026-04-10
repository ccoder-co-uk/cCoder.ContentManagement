using User = cCoder.Data.Models.Security.User;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface ITemplateRenderOrchestrationService
{
    string Render(int appId, string name, string culture, dynamic model, User user);
}
