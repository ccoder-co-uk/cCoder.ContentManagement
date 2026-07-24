// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Coordinations;

internal sealed partial class TemplateRenderCoordinationService(
    IAuthorizationBroker authorizationBroker,
    ITemplateRenderOrchestrationService templateRenderOrchestrationService) : ITemplateRenderCoordinationService
{
    private User GetCurrentUser() =>
        authorizationBroker.GetCurrentUser();

    public string Render(int appId, string name, string culture, dynamic model) =>
        TryCatch<string>(operation: () =>
    {
        ValidateRender(inputs: [appId, name, culture, model]);
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidateName(name: name, parameterName: "name");
        ValidateModel(model: model, parameterName: "model");

        culture ??= GetCurrentUser().DefaultCultureId;

        return templateRenderOrchestrationService.RenderUser(appId: appId, name: name, culture: culture, model: model, user: GetCurrentUser());

    });
}