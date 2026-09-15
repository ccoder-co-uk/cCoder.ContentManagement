// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Processings;

internal partial class JsonProcessingService
{
    private static void ValidateDeserializeItems(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateParseJsonRecordsDocument(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateSerialize(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRemovePropertiesRecursively(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}