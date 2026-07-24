// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Aggregations;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Exposures;

internal class ContentManagementPackageManager(
    IContentManagementMigrationAggregationService contentManagementMigrationAggregationService)
    : IContentManagementPackageManager
{
    public ValueTask ImportPackageAsync(int appId, Package package) =>
        contentManagementMigrationAggregationService.ImportPackageAsync(appId: appId, package: package);

    public Package ExportPackage(int appId, string packageName) =>
        contentManagementMigrationAggregationService
        .ExportPackages(appId: appId, packageNames: [packageName])
        .SingleOrDefault();
}