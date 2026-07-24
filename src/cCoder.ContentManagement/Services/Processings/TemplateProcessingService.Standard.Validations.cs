// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class TemplateProcessingService
{
    private static void ValidateTemplateOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAllTemplateOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateTemplateOnAdd(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateTemplateOnUpdate(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateOrUpdateTemplateResultOnAdd(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAllTemplateOnDelete(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}