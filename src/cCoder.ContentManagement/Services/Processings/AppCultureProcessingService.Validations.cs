// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Processings;

internal partial class AppCultureProcessingService
{
    private static void ValidateAllAppCultureOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAppCultureOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAppCultureOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdateAppCultureResultOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllAppCultureOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}