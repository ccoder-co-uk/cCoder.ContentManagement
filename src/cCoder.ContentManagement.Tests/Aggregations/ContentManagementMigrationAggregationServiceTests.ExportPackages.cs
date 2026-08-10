// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Processings;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.Data.Models.Packaging;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Aggregations;

public partial class ContentManagementMigrationAggregationServiceTests
{
    [Fact]
    public void ShouldExportEachRequestedPackageWhenExportPackages()
    {
        // Given
        Mock<IPackageExportProcessingService> packageExportProcessingServiceMock = new();

        Package rolesPackage = new() { Name = "Roles" };
        Package pagesPackage = new() { Name = "Pages" };

        packageExportProcessingServiceMock
            .Setup(expression: service => service.ExportPackage(
                appId: 7,
                packageName: "Roles"))
            .Returns(value: rolesPackage);

        packageExportProcessingServiceMock
            .Setup(expression: service => service.ExportPackage(
                appId: 7,
                packageName: "Pages"))
            .Returns(value: pagesPackage);

        ContentManagementMigrationAggregationService service = CreateService(
            packageExportProcessingService:
                packageExportProcessingServiceMock.Object);

        // When
        Package[] result = service.ExportPackages(
            appId: 7,
            packageNames: ["Roles", "Pages"]);

        // Then
        result.Should()
            .Equal(elements: [rolesPackage, pagesPackage]);

        packageExportProcessingServiceMock.VerifyAll();
    }
}