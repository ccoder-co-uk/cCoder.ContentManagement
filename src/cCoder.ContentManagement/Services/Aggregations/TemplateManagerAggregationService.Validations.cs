// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Aggregations;

internal sealed partial class TemplateManagerAggregationService
{
    private static void ValidateTemplateContentOnRead(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateHtmlOnConvert(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateTemplateOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateTemplateOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateTemplateOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateTemplateOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateTemplatesOnImport(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}