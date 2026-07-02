using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Exposures;

internal class ContentManagementPackageManager(IContentManagementMigrationAggregationService contentManagementMigrationAggregationService, IPackageOrchestrationService packageOrchestrationService) : IContentManagementPackageManager
{
    public ValueTask ImportPackageAsync(int appId, Package package) =>
        contentManagementMigrationAggregationService.ImportPackageAsync(appId, package);

    public Package ExportPackage(int appId, string packageName) =>
        packageOrchestrationService.ExportPagackages(appId, new string[1] { packageName }).SingleOrDefault();
}
