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
using Moq;
using Xunit;



namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class PackageServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenDeleteAsync()
    {
        // Given
        Guid packageId = Guid.NewGuid();
        Package package = CreateRandomPackage(id: packageId);

        packageBrokerMock.Setup(expression: x => x.GetAllPackages())
            .Returns(value: new[] { package }.AsQueryable());

        packageBrokerMock
            .Setup(
expression: x =>
                    x.DeletePackageAsync(
deletedPackage: It.Is<cCoder.Data.Models.Packaging.Package>(match: item => item.Id == package.Id)
                    )
            )
            .ReturnsAsync(value: 1);

        // When
        await packageService.DeleteAsync(packageId: packageId);

        // Then
        packageBrokerMock.Verify(expression: x => x.GetAllPackages(), times: Times.Once);

        packageBrokerMock.Verify(
expression: x =>
                x.DeletePackageAsync(
deletedPackage: It.Is<cCoder.Data.Models.Packaging.Package>(match: item => item.Id == package.Id)
                ),
times: Times.Once
        );

        packageBrokerMock.VerifyNoOtherCalls();
    }

}
