// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class CurrentAppProcessingService
{
    private static void ValidateResolveCurrentApp(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}