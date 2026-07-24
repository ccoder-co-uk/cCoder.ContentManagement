// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class ContentEventService
{
    private static void ValidateRaiseContentAddEventAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateRaiseContentUpdateEventAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateRaiseContentDeleteEventAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}