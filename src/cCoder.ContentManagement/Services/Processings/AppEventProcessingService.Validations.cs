// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Processings;

internal partial class AppEventProcessingService
{
    private static void ValidateRaiseAppAddEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaiseAppDeleteEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaiseAppUpdateEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}