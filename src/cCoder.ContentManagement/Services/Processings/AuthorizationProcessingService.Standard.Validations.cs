// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class AuthorizationProcessingService
{
    private static void ValidateIsAdminOfApp(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateResolveRenderAuthorization(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}