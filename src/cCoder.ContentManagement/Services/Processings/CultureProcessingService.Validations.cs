// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Processings;

internal partial class CultureProcessingService
{
    private static void ValidateCultureOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllCultureOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateCultureOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateCultureOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdateCultureResultOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllCultureOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}