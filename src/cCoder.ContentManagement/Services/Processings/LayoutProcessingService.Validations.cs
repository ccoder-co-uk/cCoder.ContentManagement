// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Processings;

internal partial class LayoutProcessingService
{
    private static void ValidateLayoutOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllLayoutOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateLayoutOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateLayoutOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdateLayoutResultOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllLayoutOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}