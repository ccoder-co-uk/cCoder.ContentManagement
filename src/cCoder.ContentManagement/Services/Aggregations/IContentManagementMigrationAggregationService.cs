// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Aggregations;

public interface IContentManagementMigrationAggregationService
{
    Package[] ExportPackages(int appId, string[] packageNames);

    ValueTask ImportPackageAsync(int appId, Package package);
}