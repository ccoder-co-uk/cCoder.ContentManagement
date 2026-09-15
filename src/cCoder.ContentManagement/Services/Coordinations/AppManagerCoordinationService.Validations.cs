// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Coordinations;

internal sealed partial class AppManagerCoordinationService
{
    private static void ValidateAppOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateByDomainAppOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllAppOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAdminAppOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAppWithUsersOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageOrderAppOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}