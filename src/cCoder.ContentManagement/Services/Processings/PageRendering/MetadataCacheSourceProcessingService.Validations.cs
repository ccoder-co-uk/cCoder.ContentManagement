// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Rendering.Services.Processings;

internal sealed partial class MetadataCacheSourceProcessingService
{
    private static void ValidateMetadataCacheSnapshotOnCheck(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}