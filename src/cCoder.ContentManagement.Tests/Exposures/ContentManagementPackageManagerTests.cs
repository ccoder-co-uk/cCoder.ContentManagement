// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.Data.Models.Packaging;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Exposures;

public sealed partial class ContentManagementPackageManagerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData(17)]
    public async Task ShouldDelegatePackageImportAsync(int? appId)
    {
        // Given
        Package package = new();

        Mock<IContentManagementMigrationAggregationService> aggregationService =
            new(behavior: MockBehavior.Strict);

        aggregationService
            .Setup(expression: service => service.ImportPackageAsync(
                appId: appId,
                package: package))
            .Returns(value: ValueTask.CompletedTask);

        ContentManagementPackageManager manager = new(
            contentManagementMigrationAggregationService: aggregationService.Object);

        // When
        await manager.ImportPackageAsync(appId: appId, package: package);

        // Then
        aggregationService.VerifyAll();
    }
}