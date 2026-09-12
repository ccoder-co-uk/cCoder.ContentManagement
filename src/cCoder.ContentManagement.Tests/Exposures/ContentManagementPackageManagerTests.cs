// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Services.Orchestrations;
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

        Mock<IContentManagementPackageOrchestrationService> packageOrchestrationService =
            new(behavior: MockBehavior.Strict);

        packageOrchestrationService
            .Setup(expression: service => service.ImportPackageAsync(
                appId: appId,
                package: package))
            .Returns(value: ValueTask.CompletedTask);

        ContentManagementPackageManager manager = new(
            contentManagementPackageOrchestrationService: packageOrchestrationService.Object);

        // When
        await manager.ImportPackageAsync(appId: appId, package: package);

        // Then
        packageOrchestrationService.VerifyAll();
    }
}