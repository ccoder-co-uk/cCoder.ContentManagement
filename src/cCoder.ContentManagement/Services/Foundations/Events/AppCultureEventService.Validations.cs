// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class AppCultureEventService
{
    private static void ValidateRaiseAppCultureAddEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaiseAppCultureDeleteEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}