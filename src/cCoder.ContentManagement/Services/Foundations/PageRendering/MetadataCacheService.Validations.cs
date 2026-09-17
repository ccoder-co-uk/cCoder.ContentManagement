// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class MetadataCacheService
{
    private static void ValidateMetadataCacheSnapshotOnSet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}