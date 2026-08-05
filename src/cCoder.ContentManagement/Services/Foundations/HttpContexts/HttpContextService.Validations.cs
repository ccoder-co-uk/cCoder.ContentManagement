// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Foundations.HttpContexts;

internal sealed partial class HttpContextService
{
    private static void ValidateGetPageRenderContext(
        object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}