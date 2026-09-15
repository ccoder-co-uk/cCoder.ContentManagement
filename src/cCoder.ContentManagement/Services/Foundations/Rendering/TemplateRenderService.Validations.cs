// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Foundations.Rendering;

internal sealed partial class TemplateRenderService
{
    private static void ValidateTemplateRenderFoundationOperation(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePropertyValuesTemplateRenderFoundationOperationOnGet(
        object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAppsTemplateRenderFoundationOperationOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateComponentsTemplateRenderFoundationOperationOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateResourcesTemplateRenderFoundationOperationOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateScriptsTemplateRenderFoundationOperationOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateTemplatesTemplateRenderFoundationOperationOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateComponentTemplateRenderFoundationOperationOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateScriptTemplateRenderFoundationOperationOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateResourceTemplateRenderFoundationOperationOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateMetadataTemplateRenderFoundationOperationOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateJsonPropertiesTemplateRenderFoundationOperationOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}