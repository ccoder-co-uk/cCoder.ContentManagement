// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class MarkupRenderService
{
    private static void ValidateTagHandlingOperation(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateMarkContentSecurityPolicyNonce(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateJsonPropertiesTagHandlingOperationOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}