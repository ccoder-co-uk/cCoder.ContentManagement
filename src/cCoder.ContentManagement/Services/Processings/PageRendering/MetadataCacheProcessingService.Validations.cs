// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Rendering.Services.Processings;

internal sealed partial class MetadataCacheProcessingService
{
    private static void ValidatePrepareRenderSession(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}