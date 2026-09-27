// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;
using cCoder.ContentManagement.Services.Coordinations;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Exposures;

internal class ContentManagementPackageManager(
    IContentManagementPackageCoordinationService packageCoordinationService)
    : IContentManagementPackageManager, ICompositionExposure
{
    public ValueTask ImportPackageAsync(int? appId, Package package) =>
        packageCoordinationService.ImportPackageAsync(
            appId: appId,
            package: package);

    public Package ExportPackage(int appId, string packageName) =>
        packageCoordinationService.ExportPackage(
            appId: appId,
            packageName: packageName);
}