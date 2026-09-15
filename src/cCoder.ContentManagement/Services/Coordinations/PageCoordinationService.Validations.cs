// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Coordinations;

internal partial class PageCoordinationService
{
    private static void ValidateHandlePageAddAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateHandlePageUpdateAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateHandlePageDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}