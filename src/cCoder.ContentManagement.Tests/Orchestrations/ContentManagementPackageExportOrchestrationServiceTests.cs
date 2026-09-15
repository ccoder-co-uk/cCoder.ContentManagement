// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.Packaging;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Orchestrations;

public sealed class ContentManagementPackageExportOrchestrationServiceTests
{
    [Fact]
    public void ExportPackage_WhenUserIsAppAdmin_DelegatesToExportProcessing()
    {
        // Given
        const int appId = 17;
        const string packageName = "Pages";
        Package expectedPackage = new();
        Mock<IPackageExportProcessingService> exportService = new(MockBehavior.Strict);
        Mock<IAuthorizationProcessingService> authorizationService = new(MockBehavior.Strict);

        authorizationService.Setup(expression: service =>
                service.IsAdminOfAppAuthorizationContext(
                    It.Is<AuthorizationContext>(context => context.AppId == appId)))
            .Returns(value: true);

        exportService.Setup(expression: service => service.ExportPackage(
                appId: appId,
                packageName: packageName))
            .Returns(value: expectedPackage);

        ContentManagementPackageExportOrchestrationService service = new(
            packageExportProcessingService: exportService.Object,
            authorizationProcessingService: authorizationService.Object);

        // When
        Package actualPackage = service.ExportPackage(
            appId: appId,
            packageName: packageName);

        // Then
        Assert.Same(expected: expectedPackage, actual: actualPackage);
        authorizationService.VerifyAll();
        exportService.VerifyAll();
    }
}