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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        PackageItem packageItem = CreateRandomPackageItem();

        cCoder.Data.Models.Packaging.PackageItem submitted = null;

        packageItemBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Packaging.PackageItem>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "PackageItem_update"));

        packageItemBrokerMock
            .Setup(x => x.UpdatePackageItemAsync(It.IsAny<cCoder.Data.Models.Packaging.PackageItem>()))
            .Callback<cCoder.Data.Models.Packaging.PackageItem>(candidate => submitted = candidate)
            .ReturnsAsync((cCoder.Data.Models.Packaging.PackageItem value) => value);

        // When
        PackageItem result = await packageItemService.UpdateAsync(packageItem);

        // Then
        result.Should().NotBeSameAs(packageItem);
        submitted.Should().NotBeNull();
        submitted.Should().BeEquivalentTo(packageItem);
        result.Should().BeEquivalentTo(packageItem);
        packageItemBrokerMock.Verify(
            x => x.UpdatePackageItemAsync(It.IsAny<cCoder.Data.Models.Packaging.PackageItem>()),
            Times.Once
        );
        packageItemBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Packaging.PackageItem>()), Times.AtMostOnce());
        packageItemBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "PackageItem_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        PackageItem packageItem = CreateRandomPackageItem();

        packageItemBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Packaging.PackageItem>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "PackageItem_update"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await packageItemService.UpdateAsync(packageItem);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        packageItemBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Packaging.PackageItem>()), Times.AtMostOnce());
        packageItemBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "PackageItem_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}
















