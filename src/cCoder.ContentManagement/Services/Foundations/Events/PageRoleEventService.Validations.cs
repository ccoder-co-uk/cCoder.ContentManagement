// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class PageRoleEventService
{
    private static void ValidateRaisePageRoleAddEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaisePageRoleDeleteEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}