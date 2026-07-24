// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Foundations.Authorization;

internal partial class AuthorizationService
{
    private static void ValidateAuthorize(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateIsAdmin(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateIsAdminOfApp(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}