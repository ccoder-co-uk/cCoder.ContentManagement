// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class AppCultureOrchestrationService
{
    private static void ValidateAllAppCultureOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAppCultureOnAdd(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAppCultureOnDelete(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateByAppIdOnDelete(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateOrUpdateAppCultureResultOnAdd(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAllAppCultureOnDelete(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}