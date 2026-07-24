// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class ScriptService
{
    private static void ValidateScriptOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAllScriptOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateScriptOnAdd(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateScriptOnUpdate(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}