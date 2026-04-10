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

        packageBrokerMock.Setup(x => x.GetAllPackages(false)).Returns(new[] { package }.AsQueryable());

        authorizationBrokerMock.Setup(x => x.Authorize(null, "Package_delete"));
        packageBrokerMock
            .Setup(
                x =>
                    x.DeletePackageAsync(
                        It.Is<cCoder.Data.Models.Packaging.Package>(item => item.Id == package.Id)
                    )
            )
            .ReturnsAsync(1);

        // When
        await packageService.DeleteAsync(packageId);

        // Then
        packageBrokerMock.Verify(x => x.GetAllPackages(false), Times.Once);
        packageBrokerMock.Verify(
            x =>
                x.DeletePackageAsync(
                    It.Is<cCoder.Data.Models.Packaging.Package>(item => item.Id == package.Id)
                ),
            Times.Once
        );
        packageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize(null, "Package_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Guid packageId = Guid.NewGuid();
        Package package = CreateRandomPackage(id: packageId);

        packageBrokerMock.Setup(x => x.GetAllPackages(false)).Returns(new[] { package }.AsQueryable());

        authorizationBrokerMock
            .Setup(x => x.Authorize(null, "Package_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await packageService.DeleteAsync(packageId);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        packageBrokerMock.Verify(x => x.GetAllPackages(false), Times.Once);
        packageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize(null, "Package_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

















