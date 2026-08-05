// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class PageRoleImportPersistenceProcessingService
{
    private static void ValidatePageRolesOnSynchronize(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}