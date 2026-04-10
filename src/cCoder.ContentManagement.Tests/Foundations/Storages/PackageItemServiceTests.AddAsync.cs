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

public partial class PackageItemServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        PackageItem packageItem = CreateRandomPackageItem();

        cCoder.Data.Models.Packaging.PackageItem submitted = null;

        packageItemBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Packaging.PackageItem>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "PackageItem_create"));

        packageItemBrokerMock
            .Setup(x =>
                x.AddPackageItemAsync(
                    It.Is<cCoder.Data.Models.Packaging.PackageItem>(candidate => !ReferenceEquals(candidate, packageItem))
                )
            )
            .Callback<cCoder.Data.Models.Packaging.PackageItem>(candidate => submitted = candidate)
            .ReturnsAsync((cCoder.Data.Models.Packaging.PackageItem value) => value);

        // When
        PackageItem result = await packageItemService.AddAsync(packageItem);

        // Then
        result.Should().NotBeSameAs(packageItem);
        submitted.Should().NotBeNull();

        submitted
            .Should()
            .BeEquivalentTo(packageItem, options => options.Excluding(candidate => candidate.Id));

        result
            .Should()
            .BeEquivalentTo(packageItem, options => options.Excluding(candidate => candidate.Id));

        packageItemBrokerMock.Verify(
            x =>
                x.AddPackageItemAsync(
                    It.Is<cCoder.Data.Models.Packaging.PackageItem>(candidate => !ReferenceEquals(candidate, packageItem))
                ),
            Times.Once
        );
        packageItemBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Packaging.PackageItem>()), Times.AtMostOnce());
        packageItemBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "PackageItem_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        PackageItem packageItem = CreateRandomPackageItem();

        packageItemBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Packaging.PackageItem>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "PackageItem_create"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await packageItemService.AddAsync(packageItem);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        packageItemBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Packaging.PackageItem>()), Times.AtMostOnce());
        packageItemBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "PackageItem_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}
















