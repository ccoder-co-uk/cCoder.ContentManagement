using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Processings;

public interface IPackageExportProcessingService
{
    Package ExportPackage(int appId, string packageName);
}
