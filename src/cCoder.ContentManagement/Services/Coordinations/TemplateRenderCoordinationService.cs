using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Coordinations;

internal sealed partial class TemplateRenderCoordinationService(
    IAuthorizationBroker authorizationBroker,
    ITemplateRenderOrchestrationService templateRenderOrchestrationService) : ITemplateRenderCoordinationService
{
    private User User => authorizationBroker.GetCurrentUser();

    public string Render(int appId, string name, string culture, dynamic model)
    {
        ValidateAppId(appId, "appId");
        ValidateName(name, "name");
        ValidateModel(model, "model");

        culture ??= User.DefaultCultureId;

        return templateRenderOrchestrationService.Render(appId, name, culture, model, User);
    }
}
