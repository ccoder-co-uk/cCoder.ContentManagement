// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Foundations.Rendering;

internal sealed partial class CachedPageRenderService
{
    private static void ValidateMarkContentSecurityPolicyNonce(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}