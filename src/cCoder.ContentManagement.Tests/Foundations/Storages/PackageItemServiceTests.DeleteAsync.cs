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
using System.Security;



using FluentAssertions;
using Moq;
using Xunit;



namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class PackageItemServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        Guid packageItemId = Guid.NewGuid();
        PackageItem packageItem = CreateRandomPackageItem(id: packageItemId);

        packageItemBrokerMock.Setup(expression: x => x.GetAllPackageItems())
            .Returns(value: new[] { packageItem }.AsQueryable());

        packageItemBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Packaging.PackageItem>()))
            .Returns(value: (int?)7);

        authorizationManagerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "PackageItem_delete"));

        packageItemBrokerMock
            .Setup(
expression: x =>
                    x.DeletePackageItemAsync(
deletedPackageItem: It.Is<cCoder.Data.Models.Packaging.PackageItem>(match: item => item.Id == packageItem.Id)
                    )
            )
            .ReturnsAsync(value: 1);

        // When
        await packageItemService.DeleteAsync(packageItemId: packageItemId);

        // Then
        packageItemBrokerMock.Verify(expression: x => x.GetAllPackageItems(), times: Times.Once);

        packageItemBrokerMock.Verify(
expression: x =>
                x.DeletePackageItemAsync(
deletedPackageItem: It.Is<cCoder.Data.Models.Packaging.PackageItem>(match: item => item.Id == packageItem.Id)
                ),
times: Times.Once
        );

        packageItemBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Packaging.PackageItem>()), times: Times.AtMostOnce());
        packageItemBrokerMock.VerifyNoOtherCalls();
        authorizationManagerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "PackageItem_delete"), times: Times.Once);
        authorizationManagerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Guid packageItemId = Guid.NewGuid();
        PackageItem packageItem = CreateRandomPackageItem(id: packageItemId);

        packageItemBrokerMock.Setup(expression: x => x.GetAllPackageItems())
            .Returns(value: new[] { packageItem }.AsQueryable());

        packageItemBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Packaging.PackageItem>()))
            .Returns(value: (int?)7);

        authorizationManagerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "PackageItem_delete"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await packageItemService.DeleteAsync(packageItemId: packageItemId);

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        packageItemBrokerMock.Verify(expression: x => x.GetAllPackageItems(), times: Times.Once);
        packageItemBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Packaging.PackageItem>()), times: Times.AtMostOnce());
        packageItemBrokerMock.VerifyNoOtherCalls();
        authorizationManagerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "PackageItem_delete"), times: Times.Once);
        authorizationManagerMock.VerifyNoOtherCalls();
    }

}