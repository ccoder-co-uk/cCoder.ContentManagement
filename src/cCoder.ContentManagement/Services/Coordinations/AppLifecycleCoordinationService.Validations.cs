// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Coordinations;

internal sealed partial class AppLifecycleCoordinationService
{
    private static void ValidateAppOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAppOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAppOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}