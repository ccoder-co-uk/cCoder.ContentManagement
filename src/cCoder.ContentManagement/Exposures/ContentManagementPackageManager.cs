// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Exposures;

internal class ContentManagementPackageManager(
    IContentManagementPackageOrchestrationService contentManagementPackageOrchestrationService)
    : IContentManagementPackageManager
{
    public ValueTask ImportPackageAsync(int? appId, Package package) =>
        contentManagementPackageOrchestrationService.ImportPackageAsync(
            appId: appId,
            package: package);

    public Package ExportPackage(int appId, string packageName) =>
        contentManagementPackageOrchestrationService.ExportPackage(
            appId: appId,
            packageName: packageName);
}