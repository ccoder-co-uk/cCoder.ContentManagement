// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class ComponentRenderOrchestrationService(
    IComponentRenderProcessingService componentRenderProcessingService,
    IAuthorizationProcessingService authorizationProcessingService)
        : IComponentRenderOrchestrationService
{
    public string Render(int appId, string name, string culture, string theme) =>
        TryCatch<string>(operation: () =>
    {
        ValidateRender(inputs: [appId, name, culture, theme]);
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidateName(name: name, parameterName: "name");
        ValidateTheme(theme: theme, parameterName: "theme");

        RenderAuthorization authorization = authorizationProcessingService
            .ResolveRenderAuthorization(culture: culture);

        return ExecuteRenderUser(
            appId: appId,
            name: name,
            user: authorization.User,
            culture: authorization.Culture,
            theme: theme);

    });

    public string RenderUser(int appId, string name, User user, string culture, string theme) =>
        TryCatch<string>(operation: () =>
    {
        ValidateRenderUser(inputs: [appId, name, user, culture, theme]);
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidateName(name: name, parameterName: "name");
        ValidateUser(user: user, parameterName: "user");
        ValidateTheme(theme: theme, parameterName: "theme");

        return ExecuteRenderUser(
            appId: appId,
            name: name,
            user: user,
            culture: culture,
            theme: theme);

    });

    private string ExecuteRenderUser(
        int appId,
        string name,
        User user,
        string culture,
        string theme) =>
        componentRenderProcessingService.RenderUser(
            appId: appId,
            name: name,
            user: user,
            culture: culture,
            theme: theme);

    private static void ValidateAppId(int appId, string parameterName) =>
        ThrowIf(condition: appId < 1, message: parameterName + " must be greater than 0.");

    private static void ValidateName(string name, string parameterName) =>
        ThrowIf(condition: string.IsNullOrWhiteSpace(value: name), message: parameterName + " is required.");

    private static void ValidateTheme(string theme, string parameterName) =>
        ThrowIf(condition: string.IsNullOrWhiteSpace(value: theme), message: parameterName + " is required.");

    private static User ValidateUser(User user, string parameterName)
    {
        if (user == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return user;
    }

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}