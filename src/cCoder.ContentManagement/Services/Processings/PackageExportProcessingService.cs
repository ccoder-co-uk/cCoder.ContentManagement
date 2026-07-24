// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Exports;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Processings;

internal class PackageExportProcessingService(IPackageExportService packageExportService) : IPackageExportProcessingService
{
    public Package ExportPackage(int appId, string packageName)
    {
        Package result = packageName switch
        {
            "Roles" => packageExportService.ExportRolesPackage(appId: appId),
            "Layouts" => packageExportService.ExportLayoutsPackage(appId: appId),
            "Templates" => packageExportService.ExportTemplatesPackage(appId: appId),
            "Components" => packageExportService.ExportComponentsPackage(appId: appId),
            "Scripts" => packageExportService.ExportScriptsPackage(appId: appId),
            "Resources" => packageExportService.ExportResourcesPackage(appId: appId),
            "Pages" => packageExportService.ExportPagesPackage(appId: appId),
            "PageRoles" => packageExportService.ExportPageRolesPackage(appId: appId),
            var ignoredPackage => new Package(name: packageName)
            {
                Items = new List<PackageItem>()
            },
        };

        return result;
    }
}