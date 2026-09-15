// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Processings;

internal partial class ContentProcessingService
{
    private static void ValidateContentOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllContentOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAppIdByPageIdOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateContentOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateContentOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdateContentResultOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllContentOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}