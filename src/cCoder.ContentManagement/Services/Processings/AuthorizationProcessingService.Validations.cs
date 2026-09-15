// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Processings;

internal partial class AuthorizationProcessingService
{
    private static void ValidateAuthorize(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateIsAdmin(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateResolveCurrentAuthorizationContext(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateIsAdminOfApp(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateResolveRenderAuthorization(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateUserCanPageAuthorization(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}