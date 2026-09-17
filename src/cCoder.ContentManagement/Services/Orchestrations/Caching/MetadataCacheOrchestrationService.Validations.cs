// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Orchestrations.Caching;

internal sealed partial class MetadataCacheOrchestrationService
{
    private static void ValidateMetadataOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateMetadataOnToJson(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}