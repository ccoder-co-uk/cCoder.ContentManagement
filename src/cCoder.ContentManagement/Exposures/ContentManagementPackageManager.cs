using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Orchestrations;
using Package = cCoder.Data.Models.Packaging.Package;

namespace cCoder.ContentManagement.Exposures;

internal class ContentManagementPackageManager(IContentManagementMigrationAggregationService contentManagementMigrationAggregationService, IPackageOrchestrationService packageOrchestrationService) : IContentManagementPackageManager
{
    public ValueTask ImportPackageAsync(int appId, Package package)
    {
        return contentManagementMigrationAggregationService.ImportPackageAsync(appId, package);
    }

    public Package ExportPackage(int appId, string packageName)
    {
        return packageOrchestrationService.ExportPagackages(appId, new string[1] { packageName }).SingleOrDefault();
    }
}
