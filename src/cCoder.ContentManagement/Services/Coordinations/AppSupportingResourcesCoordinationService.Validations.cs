// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Coordinations;

internal partial class AppSupportingResourcesCoordinationService
{
    private static void ValidateHandleAppAddAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateHandleAppUpdateAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateHandleAppDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}