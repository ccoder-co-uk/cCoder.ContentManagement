using Package = cCoder.Data.Models.Packaging.Package;

namespace cCoder.ContentManagement.Services.Processings;

public interface IPackageExportProcessingService
{
    Package ExportPackage(int appId, string packageName);
}
