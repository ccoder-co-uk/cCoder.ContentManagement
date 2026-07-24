// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class MetadataCacheService
{
    private static void ValidateGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}