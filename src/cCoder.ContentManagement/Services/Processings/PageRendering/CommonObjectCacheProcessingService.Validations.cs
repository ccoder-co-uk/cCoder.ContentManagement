// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Rendering.Services.Processings;

internal sealed partial class CommonObjectCacheProcessingService
{
    private static void ValidatePrepareRenderSession(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}