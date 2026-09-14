// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Processings;

internal partial class PageRoleEventProcessingService
{
    private static void ValidateRaisePageRoleAddEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaisePageRoleDeleteEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}