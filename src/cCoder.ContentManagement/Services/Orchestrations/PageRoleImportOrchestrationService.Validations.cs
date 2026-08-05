// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class PageRoleImportOrchestrationService
{
    private static void ValidatePageRoleInfosOnImport(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}