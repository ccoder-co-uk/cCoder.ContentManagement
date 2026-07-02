using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Aggregations;

public interface IContentManagementMigrationAggregationService
{
    ValueTask ImportPackageAsync(int appId, Package package);
}
