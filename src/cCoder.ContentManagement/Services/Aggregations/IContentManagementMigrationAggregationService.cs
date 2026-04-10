using Package = cCoder.Data.Models.Packaging.Package;

namespace cCoder.ContentManagement.Services.Aggregations;

public interface IContentManagementMigrationAggregationService
{
    ValueTask ImportPackageAsync(int appId, Package package);
}
