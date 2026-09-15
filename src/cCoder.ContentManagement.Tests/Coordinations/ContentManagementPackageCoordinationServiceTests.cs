// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.Packaging;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Coordinations;

public sealed class ContentManagementPackageCoordinationServiceTests
{
    [Fact]
    public async Task ImportPackageAsync_WhenAppIdIsProvided_RoutesToAppImport()
    {
        // Given
        const int appId = 17;
        Package package = new();
        Mock<IContentManagementPackageImportOrchestrationService> importService =
            new(MockBehavior.Strict);

        importService.Setup(expression: service => service.ImportAppPackageAsync(
                appId: appId,
                package: package))
            .Returns(value: ValueTask.CompletedTask);

        ContentManagementPackageCoordinationService service = CreateService(
            importService: importService.Object);

        // When
        await service.ImportPackageAsync(appId: appId, package: package);

        // Then
        importService.VerifyAll();
    }

    [Fact]
    public async Task ImportPackageAsync_WhenAppIdIsNotProvided_RoutesToCommonCacheImport()
    {
        // Given
        Package package = new();
        Mock<IContentManagementPackageImportOrchestrationService> importService =
            new(MockBehavior.Strict);

        importService.Setup(expression: service =>
                service.ImportCommonCachePackageAsync(package))
            .Returns(value: ValueTask.CompletedTask);

        ContentManagementPackageCoordinationService service = CreateService(
            importService: importService.Object);

        // When
        await service.ImportPackageAsync(appId: null, package: package);

        // Then
        importService.VerifyAll();
    }

    private static ContentManagementPackageCoordinationService CreateService(
        IContentManagementPackageImportOrchestrationService importService) =>
        new(
            packageImportOrchestrationService: importService,
            packageExportOrchestrationService:
                Mock.Of<IContentManagementPackageExportOrchestrationService>());
}