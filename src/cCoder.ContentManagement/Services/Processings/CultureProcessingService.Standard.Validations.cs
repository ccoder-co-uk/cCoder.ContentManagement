// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class CultureProcessingService
{
    private static void ValidateCultureOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAllCultureOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateCultureOnAdd(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateCultureOnUpdate(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateOrUpdateCultureResultOnAdd(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAllCultureOnDelete(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}