// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Processings;

internal partial class PageInfoEventProcessingService
{
    private static void ValidateRaisePageInfoAddEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaisePageInfoUpdateEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaisePageInfoDeleteEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}