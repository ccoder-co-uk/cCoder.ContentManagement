// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Coordinations;

internal sealed partial class CommonObjectCoordinationService
{
    private static void ValidateCommonObjectOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllCommonObjectOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateCommonObjectsOnDeserialize(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllCommonObjectsOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateCommonObjectOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdateCommonObjectResultOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllCommonObjectOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateLatestCommonObject(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}