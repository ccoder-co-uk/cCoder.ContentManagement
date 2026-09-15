// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class TemplateOrchestrationService
{
    private static void ValidateTemplateOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllTemplateOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateTemplateOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateTemplateOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateByAppIdOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdateTemplateResultOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateImportTemplatesAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllTemplateOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}