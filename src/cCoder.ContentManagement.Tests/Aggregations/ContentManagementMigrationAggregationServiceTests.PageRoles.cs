// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Aggregations;

public partial class ContentManagementMigrationAggregationServiceTests
{
    [Fact]
    public async Task ShouldRaisePageRolesPackageEventAfterPagesAreImportedAsync()
    {
        // Given
        const int appId = 7;
        MockSequence sequence = new();
        Mock<IPageOrchestrationService> pageServiceMock = new(behavior: MockBehavior.Strict);
        Mock<IPackageOrchestrationService> packageServiceMock = new(behavior: MockBehavior.Strict);

        pageServiceMock
            .InSequence(sequence: sequence)
            .Setup(expression: service => service.ImportPagesAsync(
                appId: appId,
                items: It.IsAny<Page[]>()))
            .ReturnsAsync(value: Array.Empty<Page>());

        packageServiceMock
            .InSequence(sequence: sequence)
            .Setup(expression: service => service.RaisePackagePageRolesImportEventAsync(
                appId: appId,
                package: It.Is<Package>(match: package =>
                    package.Items.Count == 2
                    && package.Items.Any(predicate: item => item.Type == "ContentManagement/Page")
                    && package.Items.Any(predicate: item => item.Type == "ContentManagement/PageRole"))))
            .Returns(value: ValueTask.CompletedTask);

        packageServiceMock
            .InSequence(sequence: sequence)
            .Setup(expression: service => service.RaisePackageImportCompleteEventAsync(
                appId: appId,
                package: It.IsAny<Package>()))
            .Returns(value: ValueTask.CompletedTask);

        ContentManagementMigrationAggregationService service = CreateService(
            pageOrchestrationService: pageServiceMock.Object,
            packageOrchestrationService: packageServiceMock.Object);

        Package package = CreatePageRolePackage();

        // When
        await service.ImportPackageAsync(appId: appId, package: package);

        // Then
        pageServiceMock.Verify(
            expression: service => service.ImportPagesAsync(
                appId: appId,
                items: It.IsAny<Page[]>()),
            times: Times.Once);

        packageServiceMock.Verify(
            expression: service => service.RaisePackagePageRolesImportEventAsync(
                appId: appId,
                package: It.IsAny<Package>()),
            times: Times.Once);

        packageServiceMock.Verify(
            expression: service => service.RaisePackageImportCompleteEventAsync(
                appId: appId,
                package: It.IsAny<Package>()),
            times: Times.Once);

        pageServiceMock.VerifyNoOtherCalls();
        packageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldNotRaisePageRolesPackageEventWhenPageImportFailsAsync()
    {
        // Given
        const int appId = 7;
        Mock<IPageOrchestrationService> pageServiceMock = new(behavior: MockBehavior.Strict);
        Mock<IPackageOrchestrationService> packageServiceMock = new(behavior: MockBehavior.Strict);

        pageServiceMock
            .Setup(expression: service => service.ImportPagesAsync(
                appId: appId,
                items: It.IsAny<Page[]>()))
            .Throws(exception: new InvalidOperationException(message: "Page import failed."));

        ContentManagementMigrationAggregationService service = CreateService(
            pageOrchestrationService: pageServiceMock.Object,
            packageOrchestrationService: packageServiceMock.Object);

        Package package = CreatePageRolePackage();

        // When
        ValueTask action() => service.ImportPackageAsync(appId: appId, package: package);

        // Then
        _ = await Assert.ThrowsAnyAsync<Exception>(testCode: async () => await action());
        packageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldNotCompletePackageWhenPageChildImportFailsAsync()
    {
        // Given
        const int appId = 7;
        Page importedPage = new() { Id = 11, AppId = appId, Name = "Home", Path = string.Empty };
        Mock<IPageOrchestrationService> pageServiceMock = new(behavior: MockBehavior.Strict);
        Mock<IPageImportOrchestrationService> pageImportServiceMock = new(behavior: MockBehavior.Strict);
        Mock<IPackageOrchestrationService> packageServiceMock = new(behavior: MockBehavior.Strict);

        pageServiceMock
            .Setup(expression: service => service.ImportPagesAsync(
                appId: appId,
                items: It.IsAny<Page[]>()))
            .ReturnsAsync(value: [importedPage]);

        pageImportServiceMock
            .Setup(expression: service => service.HandlePageImportAsync(
                page: importedPage))
            .Throws(exception: new InvalidOperationException(message: "Page child import failed."));

        ContentManagementMigrationAggregationService service = CreateService(
            pageOrchestrationService: pageServiceMock.Object,
            pageImportOrchestrationService: pageImportServiceMock.Object,
            packageOrchestrationService: packageServiceMock.Object);

        Package package = CreatePageRolePackage();

        // When
        ValueTask action() => service.ImportPackageAsync(appId: appId, package: package);

        // Then
        _ = await Assert.ThrowsAnyAsync<Exception>(testCode: async () => await action());
        packageServiceMock.VerifyNoOtherCalls();
    }

    private static Package CreatePageRolePackage() =>
        new()
        {
            Items =
            [
                new PackageItem
                {
                    Type = "ContentManagement/Page",
                    Data = "[{\"Name\":\"Home\",\"Path\":\"\"}]",
                },
                new PackageItem
                {
                    Type = "ContentManagement/PageRole",
                    Data = "[{\"Path\":\"\",\"Role\":\"Guests\"}]",
                },
            ],
        };
}