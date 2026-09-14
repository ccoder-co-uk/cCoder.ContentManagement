// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Foundations.Serialization;

internal partial class JsonService
{
    private static void ValidateJsonOnParse(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateValueOnSerialize(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateJsonObjectOnCheck(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateJsonArrayOnCheck(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateJsonPropertiesOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateJsonItemsOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateJsonPropertyOnRemove(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateJsonRecordsDocumentOnParse(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeserialize(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}