// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Coordinations;

internal sealed partial class ComponentRenderCoordinationService(
    IAuthorizationBroker authorizationBroker,
    IComponentRenderOrchestrationService componentRenderOrchestrationService) : IComponentRenderCoordinationService
{
    private User GetCurrentUser() =>
        authorizationBroker.GetCurrentUser();

    public string Render(int appId, string name, string culture, string theme) =>
        TryCatch<string>(operation: () =>
    {
        ValidateRender(inputs: [appId, name, culture, theme]);
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidateName(name: name, parameterName: "name");
        ValidateTheme(theme: theme, parameterName: "theme");

        culture ??= GetCurrentUser().DefaultCultureId;

        return componentRenderOrchestrationService.RenderUser(appId: appId, name: name, user: GetCurrentUser(), culture: culture, theme: theme);

    });
}