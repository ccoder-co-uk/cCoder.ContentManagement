// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Coordinations;

internal sealed partial class ContentManagementPackageCoordinationService(
    IContentManagementPackageImportOrchestrationService packageImportOrchestrationService,
    IContentManagementPackageExportOrchestrationService packageExportOrchestrationService)
    : IContentManagementPackageCoordinationService
{
    public ValueTask ImportPackageAsync(int? appId, Package package) =>
        TryCatch(operation: async () =>
    {
        ValidateImportPackage(inputs: [appId, package]);

        if (appId.HasValue)
        {
            await packageImportOrchestrationService.ImportAppPackageAsync(
                appId: appId.Value,
                package: package);
        }
        else
        {
            await packageImportOrchestrationService.ImportCommonCachePackageAsync(
                package: package);
        }
    }, isValueTask: true);

    public Package ExportPackage(int appId, string packageName) =>
        TryCatch(operation: () =>
    {
        ValidateExportPackage(inputs: [appId, packageName]);

        return packageExportOrchestrationService.ExportPackage(
            appId: appId,
            packageName: packageName);
    });
}