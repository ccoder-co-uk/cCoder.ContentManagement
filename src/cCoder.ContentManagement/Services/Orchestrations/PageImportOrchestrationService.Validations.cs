// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class PageImportOrchestrationService
{
    private static void ValidateHandlePageImportAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}