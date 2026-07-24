// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.Config;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using System.Security;



using FluentAssertions;
using Moq;
using Xunit;



namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class PackageServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        Guid packageId = Guid.NewGuid();
        Package package = CreateRandomPackage(id: packageId);

        packageBrokerMock.Setup(expression: x => x.GetAllPackages(ignoreFilters: false))
            .Returns(value: new[] { package }.AsQueryable());

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: null, privilege: "Package_delete"));

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
        packageBrokerMock.Verify(expression: x => x.GetAllPackages(ignoreFilters: false), times: Times.Once);

        packageBrokerMock.Verify(
expression: x =>
                x.DeletePackageAsync(
deletedPackage: It.Is<cCoder.Data.Models.Packaging.Package>(match: item => item.Id == package.Id)
                ),
times: Times.Once
        );

        packageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: null, privilege: "Package_delete"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Guid packageId = Guid.NewGuid();
        Package package = CreateRandomPackage(id: packageId);

        packageBrokerMock.Setup(expression: x => x.GetAllPackages(ignoreFilters: false))
            .Returns(value: new[] { package }.AsQueryable());

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: null, privilege: "Package_delete"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await packageService.DeleteAsync(packageId: packageId);

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        packageBrokerMock.Verify(expression: x => x.GetAllPackages(ignoreFilters: false), times: Times.Once);
        packageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: null, privilege: "Package_delete"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}