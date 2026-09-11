// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Foundations.Serialization;

internal partial class JsonService
{
    private static void ValidateJsonOnParse(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateValueOnSerialize(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateJsonObjectOnCheck(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateJsonArrayOnCheck(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateJsonPropertiesOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateJsonItemsOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateJsonPropertyOnRemove(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateJsonRecordsDocumentOnParse(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateDeserialize(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}