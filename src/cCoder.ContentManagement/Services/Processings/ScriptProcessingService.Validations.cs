// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Processings;

internal partial class ScriptProcessingService
{
    private static void ValidateScriptOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllScriptOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateScriptOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateScriptOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdateScriptResultOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllScriptOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}