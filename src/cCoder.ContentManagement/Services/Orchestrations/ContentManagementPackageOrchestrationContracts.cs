// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal interface IContentManagementPackageImportOrchestrationService
{
    ValueTask ImportCommonCachePackageAsync(Package package);

    ValueTask ImportAppPackageAsync(int appId, Package package);
}

internal interface IContentManagementPackageExportOrchestrationService
{
    Package ExportPackage(int appId, string packageName);
}