// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class CultureEventService
{
    private static void ValidateRaiseCultureAddEventAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateRaiseCultureUpdateEventAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateRaiseCultureDeleteEventAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}