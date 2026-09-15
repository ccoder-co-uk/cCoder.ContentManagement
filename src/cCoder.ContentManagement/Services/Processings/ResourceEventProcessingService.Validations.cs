// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Processings;

internal partial class ResourceEventProcessingService
{
    private static void ValidateRaiseResourceAddEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaiseResourceUpdateEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaiseResourceDeleteEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}