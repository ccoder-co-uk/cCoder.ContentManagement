// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Services.Coordinations;
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

        Mock<IContentManagementPackageCoordinationService> packageOrchestrationService =
            new(behavior: MockBehavior.Strict);

        packageOrchestrationService
            .Setup(expression: service => service.ImportPackageAsync(
                appId: appId,
                package: package))
            .Returns(value: ValueTask.CompletedTask);

        ContentManagementPackageManager manager = new(
            packageCoordinationService: packageOrchestrationService.Object);

        // When
        await manager.ImportPackageAsync(appId: appId, package: package);

        // Then
        packageOrchestrationService.VerifyAll();
    }

    [Fact]
    public void ShouldDelegatePackageExport()
    {
        // Given
        const int appId = 17;
        const string packageName = "Pages";
        Package expectedPackage = new();

        Mock<IContentManagementPackageCoordinationService> exportService =
            new(behavior: MockBehavior.Strict);

        exportService.Setup(expression: service => service.ExportPackage(
                appId: appId,
                packageName: packageName))
            .Returns(value: expectedPackage);

        ContentManagementPackageManager manager = new(
            packageCoordinationService: exportService.Object);

        // When
        Package actualPackage = manager.ExportPackage(
            appId: appId,
            packageName: packageName);

        // Then
        Assert.Same(expected: expectedPackage, actual: actualPackage);
        exportService.VerifyAll();
    }
}