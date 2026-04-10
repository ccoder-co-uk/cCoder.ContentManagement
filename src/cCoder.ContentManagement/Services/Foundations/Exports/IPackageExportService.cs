using Package = cCoder.Data.Models.Packaging.Package;

namespace cCoder.ContentManagement.Services.Foundations.Exports;

public interface IPackageExportService
{
    Package ExportRoles(int appId);

    Package ExportLayouts(int appId);

    Package ExportTemplates(int appId);

    Package ExportComponents(int appId);

    Package ExportScripts(int appId);

    Package ExportResources(int appId);

    Package ExportPages(int appId);

    Package ExportPageRoles(int appId);
}
