// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.ContentManagement.Models;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed class TemplateRenderOrchestrationService(
    ITemplateRenderProcessingService templateRenderProcessingService,
    Config config,
    ILogger<TemplateRenderOrchestrationService> log) : ITemplateRenderOrchestrationService
{
    public string Render(int appId, string name, string culture, dynamic model, User user)
    {
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidateTemplateName(name: name, parameterName: "name");
        ValidateUser(user: user, parameterName: "user");

        return templateRenderProcessingService.Render(appId: appId, name: name, model: model, user: user, culture: culture, config: config, log: log);
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