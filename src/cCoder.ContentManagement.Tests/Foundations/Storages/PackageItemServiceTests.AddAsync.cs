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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        PackageItem packageItem = CreateRandomPackageItem();

        cCoder.Data.Models.Packaging.PackageItem submitted = null;

        packageItemBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Packaging.PackageItem>()))
            .Returns(value: (int?)7);

        authorizationManagerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "PackageItem_create"));

        packageItemBrokerMock
            .Setup(expression: x =>
                x.AddPackageItemAsync(
newPackageItem: It.Is<cCoder.Data.Models.Packaging.PackageItem>(match: candidate => !ReferenceEquals(objA: candidate, objB: packageItem))
                )
            )
            .Callback<cCoder.Data.Models.Packaging.PackageItem>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (cCoder.Data.Models.Packaging.PackageItem value) => value);

        // When
        PackageItem result = await packageItemService.AddPackageItemAsync(newPackageItem: packageItem);

        // Then

        result.Should()
            .BeSameAs(expected: packageItem);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: packageItem);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted
            .Should()
            .BeEquivalentTo(expectation: packageItem, config: options => options.Excluding(expression: candidate => candidate.Id));

        result
            .Should()
            .BeEquivalentTo(expectation: packageItem, config: options => options.Excluding(expression: candidate => candidate.Id));

        packageItemBrokerMock.Verify(
expression: x =>
                x.AddPackageItemAsync(
newPackageItem: It.Is<cCoder.Data.Models.Packaging.PackageItem>(match: candidate => !ReferenceEquals(objA: candidate, objB: packageItem))
                ),
times: Times.Once
        );

        packageItemBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Packaging.PackageItem>()), times: Times.AtMostOnce());
        packageItemBrokerMock.VerifyNoOtherCalls();
        authorizationManagerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "PackageItem_create"), times: Times.Once);
        authorizationManagerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        PackageItem packageItem = CreateRandomPackageItem();

        packageItemBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Packaging.PackageItem>()))
            .Returns(value: (int?)7);

        authorizationManagerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "PackageItem_create"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await packageItemService.AddPackageItemAsync(newPackageItem: packageItem);

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        packageItemBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Packaging.PackageItem>()), times: Times.AtMostOnce());
        packageItemBrokerMock.VerifyNoOtherCalls();
        authorizationManagerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "PackageItem_create"), times: Times.Once);
        authorizationManagerMock.VerifyNoOtherCalls();
    }

}