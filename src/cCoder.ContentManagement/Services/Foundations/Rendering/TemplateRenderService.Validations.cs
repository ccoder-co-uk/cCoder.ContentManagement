// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Foundations.Rendering;

internal sealed partial class TemplateRenderService
{
    private static void ValidateTemplateRenderFoundationOperation(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAppsTemplateRenderFoundationOperationOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateComponentsTemplateRenderFoundationOperationOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateResourcesTemplateRenderFoundationOperationOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateScriptsTemplateRenderFoundationOperationOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateTemplatesTemplateRenderFoundationOperationOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateComponentTemplateRenderFoundationOperationOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateScriptTemplateRenderFoundationOperationOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateResourceTemplateRenderFoundationOperationOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateMetadataTemplateRenderFoundationOperationOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateJsonPropertiesTemplateRenderFoundationOperationOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}