// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;
using cCoder.Data.Models.CMS;
using System.ComponentModel.DataAnnotations;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class TemplateService
{
    private static void ValidateId(int templateId, string parameterName) =>
        ThrowIf(condition: templateId < 1, message: parameterName + " must be greater than 0.");

    private static void ValidateTemplate(Template template, string parameterName)
    {
        if (template == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        if (template.AppId < 1)
        {
            throw new ValidationException(message: parameterName + ".AppId must be greater than 0.");
        }

        if (string.IsNullOrWhiteSpace(value: template.Name))
        {
            throw new ValidationException(message: parameterName + ".Name is required.");
        }
    }

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }

    private static void ValidateTemplateOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAllTemplateOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateTemplateOnAdd(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateTemplateOnUpdate(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}