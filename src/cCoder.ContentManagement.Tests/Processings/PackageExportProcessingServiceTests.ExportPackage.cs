// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.ContentManagementConfiguration;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using FluentAssertions;
using Xunit;



namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PackageExportProcessingServiceTests
{
    [Theory]
    [InlineData("Roles")]
    [InlineData("Layouts")]
    [InlineData("Templates")]
    [InlineData("Components")]
    [InlineData("Scripts")]
    [InlineData("Resources")]
    [InlineData("Pages")]
    [InlineData("PageRoles")]
    public void ShouldDispatchToMatchingExportWhenKnownPackageIsRequestedForExportPackage(
        string packageName
    )
    {
        // Given
        const int appId = 1;
        Package expectedPackage = CreatePackage(name: packageName);

        SetupKnownPackageExport(appId: appId, packageName: packageName, expectedPackage: expectedPackage);

        // When
        Package result = processingService.ExportPackage(appId: appId, packageName: packageName);

        // Then

        result.Should()
            .BeSameAs(expected: expectedPackage);

        packageExportServiceMock.VerifyAll();
        packageExportServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldReturnEmptyPackageWhenPackageIsUnknownForExportPackage()
    {
        // Given
        const int appId = 1;

        // When
        Package result = processingService.ExportPackage(appId: appId, packageName: "Unknown");

        // Then

        result.Name.Should()
            .Be(expected: "Unknown");

        result.Items.Should()
            .BeEmpty();

        packageExportServiceMock.VerifyNoOtherCalls();
    }

}