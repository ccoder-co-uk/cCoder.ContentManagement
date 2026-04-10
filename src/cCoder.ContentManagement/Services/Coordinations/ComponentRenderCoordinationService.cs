using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Services.Orchestrations;
using User = cCoder.Data.Models.Security.User;

namespace cCoder.ContentManagement.Services.Coordinations;

internal sealed partial class ComponentRenderCoordinationService(
    IAuthorizationBroker authorizationBroker,
    IComponentRenderOrchestrationService componentRenderOrchestrationService) : IComponentRenderCoordinationService
{
    private User User => authorizationBroker.GetCurrentUser();

    public string Render(int appId, string name, string culture, string theme)
    {
        ValidateAppId(appId, "appId");
        ValidateName(name, "name");
        ValidateTheme(theme, "theme");

        culture ??= User.DefaultCultureId;

        return componentRenderOrchestrationService.Render(appId, name, User, culture, theme);
    }
}
