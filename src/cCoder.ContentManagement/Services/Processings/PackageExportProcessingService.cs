using cCoder.ContentManagement.Services.Foundations.Exports;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Processings;

internal class PackageExportProcessingService(IPackageExportService packageExportService) : IPackageExportProcessingService
{
    public Package ExportPackage(int appId, string packageName)
    {
        Package result = packageName switch
        {
            "Roles" => packageExportService.ExportRoles(appId),
            "Layouts" => packageExportService.ExportLayouts(appId),
            "Templates" => packageExportService.ExportTemplates(appId),
            "Components" => packageExportService.ExportComponents(appId),
            "Scripts" => packageExportService.ExportScripts(appId),
            "Resources" => packageExportService.ExportResources(appId),
            "Pages" => packageExportService.ExportPages(appId),
            "PageRoles" => packageExportService.ExportPageRoles(appId),
            var ignoredPackage => new Package(packageName)
            {
                Items = new List<PackageItem>()
            },
        };

        return result;
    }
}
