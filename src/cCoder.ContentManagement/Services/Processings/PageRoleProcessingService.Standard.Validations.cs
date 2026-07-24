// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class PageRoleProcessingService
{
    private static void ValidateAllPageRoleOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidatePageRoleOnAdd(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidatePageRoleOnDelete(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateOrUpdatePageRoleResultOnAdd(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateImportPageRolesAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAllPageRoleOnDelete(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}