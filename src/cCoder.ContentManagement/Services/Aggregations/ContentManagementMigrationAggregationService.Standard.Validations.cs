// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Aggregations;

internal partial class ContentManagementMigrationAggregationService
{
    private static void ValidateImportPackageAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}