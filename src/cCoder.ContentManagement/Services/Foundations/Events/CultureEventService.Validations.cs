// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class CultureEventService
{
    private static void ValidateRaiseCultureAddEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaiseCultureUpdateEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaiseCultureDeleteEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}