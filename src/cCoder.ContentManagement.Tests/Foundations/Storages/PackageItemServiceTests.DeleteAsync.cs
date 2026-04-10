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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        Guid packageItemId = Guid.NewGuid();
        PackageItem packageItem = CreateRandomPackageItem(id: packageItemId);

        packageItemBrokerMock.Setup(x => x.GetAllPackageItems(false)).Returns(new[] { packageItem }.AsQueryable());

        packageItemBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Packaging.PackageItem>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "PackageItem_delete"));
        packageItemBrokerMock
            .Setup(
                x =>
                    x.DeletePackageItemAsync(
                        It.Is<cCoder.Data.Models.Packaging.PackageItem>(item => item.Id == packageItem.Id)
                    )
            )
            .ReturnsAsync(1);

        // When
        await packageItemService.DeleteAsync(packageItemId);

        // Then
        packageItemBrokerMock.Verify(x => x.GetAllPackageItems(false), Times.Once);
        packageItemBrokerMock.Verify(
            x =>
                x.DeletePackageItemAsync(
                    It.Is<cCoder.Data.Models.Packaging.PackageItem>(item => item.Id == packageItem.Id)
                ),
            Times.Once
        );
        packageItemBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Packaging.PackageItem>()), Times.AtMostOnce());
        packageItemBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "PackageItem_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Guid packageItemId = Guid.NewGuid();
        PackageItem packageItem = CreateRandomPackageItem(id: packageItemId);

        packageItemBrokerMock.Setup(x => x.GetAllPackageItems(false)).Returns(new[] { packageItem }.AsQueryable());

        packageItemBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Packaging.PackageItem>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "PackageItem_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await packageItemService.DeleteAsync(packageItemId);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        packageItemBrokerMock.Verify(x => x.GetAllPackageItems(false), Times.Once);
        packageItemBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Packaging.PackageItem>()), Times.AtMostOnce());
        packageItemBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "PackageItem_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

















