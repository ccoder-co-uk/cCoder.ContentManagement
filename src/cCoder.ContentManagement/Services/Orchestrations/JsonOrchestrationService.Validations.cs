// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class JsonOrchestrationService
{
    private static void ValidateSerializeRuntimeValue(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateParseJsonRecordsDocument(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}