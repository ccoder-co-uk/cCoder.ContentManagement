// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.ContentManagement.Models;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class TemplateRenderOrchestrationService(
    ITemplateRenderProcessingService templateRenderProcessingService,
    IAuthorizationProcessingService authorizationProcessingService)
        : ITemplateRenderOrchestrationService
{
    public TemplateRenderResult RenderTemplateRenderResult(
        int appId,
        string name,
        string culture,
        dynamic model) =>
        TryCatch<TemplateRenderResult>(operation: () =>
    {
        ValidateRender(inputs: [appId, name, culture, model]);
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidateTemplateName(name: name, parameterName: "name");

        RenderAuthorization authorization = authorizationProcessingService
            .ResolveRenderAuthorizationContext(
                context: new AuthorizationContext
                {
                    Culture = culture
                })
            .RenderAuthorization;

        return ExecuteRenderUser(
            appId: appId,
            name: name,
            culture: authorization.Culture,
            model: model,
            user: authorization.User);

    });

    internal string Render(
        int appId,
        string name,
        string culture,
        dynamic model) =>
        RenderTemplateRenderResult(
            appId: appId,
            name: name,
            culture: culture,
            model: model).Content;

    internal string RenderUser(
        int appId,
        string name,
        string culture,
        dynamic model,
        User user) =>
        TryCatch<TemplateRenderResult>(operation: () =>
    {
        ValidateRenderUser(inputs: [appId, name, culture, model, user]);
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidateTemplateName(name: name, parameterName: "name");
        ValidateUser(user: user, parameterName: "user");

        return ExecuteRenderUser(
            appId: appId,
            name: name,
            culture: culture,
            model: model,
            user: user);

    }).Content;

    private TemplateRenderResult ExecuteRenderUser(
        int appId,
        string name,
        string culture,
        dynamic model,
        User user)
    {
        TemplateRenderOperation operation = new()
        {
            AppId = appId,
            Name = name,
            Model = model,
            User = user,
            Culture = culture
        };

        return new TemplateRenderResult
        {
            Content = templateRenderProcessingService
                .RenderTemplateRenderOperation(operation: operation),
            StatusCode = StatusCodes.Status200OK
        };
    }

    private static void ValidateAppId(int appId, string parameterName) =>
        ThrowIf(condition: appId < 1, message: parameterName + " must be greater than 0.");

    private static void ValidateTemplateName(string name, string parameterName) =>
        ThrowIf(condition: string.IsNullOrWhiteSpace(value: name), message: parameterName + " is required.");

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