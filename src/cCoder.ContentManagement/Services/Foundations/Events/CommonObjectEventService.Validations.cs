// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class CommonObjectEventService
{
    private static void ValidateRaiseCommonObjectAddEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaiseCommonObjectUpdateEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaiseCommonObjectDeleteEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaiseCommonObjectsImportedEventAsync(
        object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}