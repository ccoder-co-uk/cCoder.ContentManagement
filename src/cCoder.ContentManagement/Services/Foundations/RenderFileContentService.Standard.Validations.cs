// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Foundations;

internal partial class RenderFileContentService
{
    private static void ValidateLatestTextContentOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}