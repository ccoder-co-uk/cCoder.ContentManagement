using Package = cCoder.Data.Models.Packaging.Package;

namespace cCoder.ContentManagement.Exposures;

public interface IContentManagementPackageManager
{
    ValueTask ImportPackageAsync(int appId, Package package);

    Package ExportPackage(int appId, string packageName);
}
