// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class LayoutEventService
{
    private static void ValidateRaiseLayoutAddEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaiseLayoutUpdateEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaiseLayoutDeleteEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}