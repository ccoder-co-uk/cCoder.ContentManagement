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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        Package package = CreateRandomPackage();

        cCoder.Data.Models.Packaging.Package submitted = null;

        authorizationBrokerMock.Setup(x => x.Authorize(null, "Package_create"));

        packageBrokerMock
            .Setup(x =>
                x.AddPackageAsync(It.Is<cCoder.Data.Models.Packaging.Package>(candidate => !ReferenceEquals(candidate, package)))
            )
            .Callback<cCoder.Data.Models.Packaging.Package>(candidate => submitted = candidate)
            .ReturnsAsync((cCoder.Data.Models.Packaging.Package value) => value);

        // When
        Package result = await packageService.AddAsync(package);

        // Then
        result.Should().BeSameAs(package);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(package);
        result.Should().NotBeSameAs(submitted);

        submitted
            .Should()
            .BeEquivalentTo(
                new
                {
                    package.Id,
                    package.Name,
                    package.Description,
                    package.Category,
                    package.SourceApi
                });
        submitted.Items.Should().BeNull();

        result
            .Should()
            .BeEquivalentTo(
                new
                {
                    package.Id,
                    package.Name,
                    package.Description,
                    package.Category,
                    package.SourceApi
                });
        result.Items.Should().BeEquivalentTo(package.Items);

        packageBrokerMock.Verify(
            x =>
                x.AddPackageAsync(
                    It.Is<cCoder.Data.Models.Packaging.Package>(candidate => !ReferenceEquals(candidate, package))
                ),
            Times.Once
        );
        packageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize(null, "Package_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        Package package = CreateRandomPackage();

        authorizationBrokerMock
            .Setup(x => x.Authorize(null, "Package_create"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await packageService.AddAsync(package);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        packageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize(null, "Package_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}
















