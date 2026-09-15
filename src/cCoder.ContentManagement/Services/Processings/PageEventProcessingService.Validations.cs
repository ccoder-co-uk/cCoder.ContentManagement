// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Processings;

internal partial class PageEventProcessingService
{
    private static void ValidateRaisePageAddEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaisePageUpdateEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaisePageDeleteEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}