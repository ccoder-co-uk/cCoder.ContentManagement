// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Processings;

internal partial class AppProcessingService
{
    private static void ValidateAppForRenderOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAppForDeleteOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAppOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDomainOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateByDomainAppOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllAppOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAppOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAppOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdateAppResultOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllAppOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateResolveCurrentApp(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageOrderAppOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}