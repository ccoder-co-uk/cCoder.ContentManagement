// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class PageRoleImportLookupProcessingService
{
    private static void ValidatePageRoleOnResolve(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}