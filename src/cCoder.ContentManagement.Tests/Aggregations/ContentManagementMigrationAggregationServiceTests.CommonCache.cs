// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models;
using cCoder.Data.Models.Packaging;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Aggregations;

public partial class ContentManagementMigrationAggregationServiceTests
{
    [Fact]
    public async Task ImportPackageAsync_ShouldImportCommonObjectsAsync()
    {
        // Given
        Mock<ICommonObjectOrchestrationService> commonObjectService =
            new(behavior: MockBehavior.Strict);

        Package package = new()
        {
            Category = "Baseline",
            Items =
            [
                new PackageItem
                {
                    Type = "Core/Component",
                    Data = """
                           {
                             "Name": "Navigation",
                             "ResourceKey": "Core",
                             "Content": "<nav></nav>",
                             "CreatedOn": "2026-08-10T12:00:00Z",
                             "LastUpdated": "2026-08-10T12:05:00Z"
                           }
                           """
                }
            ]
        };

        commonObjectService
            .Setup(expression: service => service.AddAllCommonObjectsAsync(
                newCommonObjects: It.Is<CommonObject[]>(match: items =>
                    items.Length == 1
                    && items[0].Name == "Navigation"
                    && items[0].Key == "Core"
                    && items[0].Type == "Core/Component")))
            .ReturnsAsync(value:
            [
                new OperationResult<CommonObject>
                {
                    Success = true
                }
            ]);

        ContentManagementMigrationAggregationService service = CreateService(
            commonObjectOrchestrationService: commonObjectService.Object);

        // When
        await service.ImportPackageAsync(appId: null, package: package);

        // Then
        commonObjectService.VerifyAll();

        commonObjectService.Invocations.Should()
            .ContainSingle();
    }
}