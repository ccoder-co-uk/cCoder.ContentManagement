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
            "Roles" => packageExportService.ExportRoles(appId: appId),
            "Layouts" => packageExportService.ExportLayouts(appId: appId),
            "Templates" => packageExportService.ExportTemplates(appId: appId),
            "Components" => packageExportService.ExportComponents(appId: appId),
            "Scripts" => packageExportService.ExportScripts(appId: appId),
            "Resources" => packageExportService.ExportResources(appId: appId),
            "Pages" => packageExportService.ExportPages(appId: appId),
            "PageRoles" => packageExportService.ExportPageRoles(appId: appId),
            var ignoredPackage => new Package(name: packageName)
            {
                Items = new List<PackageItem>()
            },
        };

        return result;
    }
}