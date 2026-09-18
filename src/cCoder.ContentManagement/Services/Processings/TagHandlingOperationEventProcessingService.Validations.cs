// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Processings;

internal partial class TagHandlingOperationEventProcessingService
{
    private static void ValidateRaiseTagHandlingOperationRenderTagsAsync(
        object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}