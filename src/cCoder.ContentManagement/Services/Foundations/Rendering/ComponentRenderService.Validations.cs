// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace cCoder.ContentManagement.Services.Foundations.Rendering;

internal sealed partial class ComponentRenderService
{
    private static void ValidateComponentRenderFoundationOperation(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAppsComponentRenderFoundationOperationOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateComponentsComponentRenderFoundationOperationOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateResourcesComponentRenderFoundationOperationOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateScriptsComponentRenderFoundationOperationOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateComponentComponentRenderFoundationOperationOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateScriptComponentRenderFoundationOperationOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateResourceComponentRenderFoundationOperationOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateMetadataComponentRenderFoundationOperationOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateLatestTextContentComponentRenderFoundationOperationOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateJsonPropertiesComponentRenderFoundationOperationOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static int ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
        {
            throw new ValidationException(
                message: parameterName + " must be greater than 0.");
        }

        return appId;
    }

    private static string ValidatePath(string path, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value: path))
        {
            throw new ValidationException(
                message: parameterName + " is required.");
        }

        return path;
    }

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}