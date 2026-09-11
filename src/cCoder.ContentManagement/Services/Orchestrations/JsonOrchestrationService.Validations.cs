// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class JsonOrchestrationService
{
    private static void ValidateSerializeRuntimeValue(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateParseJsonRecordsDocument(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}