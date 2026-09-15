// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Foundations.Authorization;

internal partial class AuthorizationService
{
    private static void ValidateAppWithRolesOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRolesForUserOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateUserWithRolesOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}