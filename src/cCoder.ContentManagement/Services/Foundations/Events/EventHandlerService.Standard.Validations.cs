// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class EventHandlerService
{
    private static void ValidateListenToAllEvents(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}