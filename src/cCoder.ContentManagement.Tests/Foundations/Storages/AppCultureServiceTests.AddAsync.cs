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
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class AppCultureServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        AppCulture appCulture = CreateRandomAppCulture();

        CmsDataModels.AppCulture submitted = null;

        authorizationBrokerMock.Setup(x =>
            x.Authorize((int?)appCulture.AppId, "AppCulture_create")
        );

        appCultureBrokerMock
            .Setup(x =>
                x.AddAppCultureAsync(
                    It.Is<CmsDataModels.AppCulture>(candidate => !ReferenceEquals(candidate, appCulture))
                )
            )
            .Callback<CmsDataModels.AppCulture>(candidate => submitted = candidate)
            .ReturnsAsync((CmsDataModels.AppCulture value) => value);

        // When
        AppCulture result = await appCultureService.AddAsync(appCulture);

        // Then
        result.Should().NotBeSameAs(appCulture);
        submitted.Should().NotBeNull();
        submitted.Should().BeEquivalentTo(appCulture);
        result.Should().BeEquivalentTo(appCulture);
        appCultureBrokerMock.Verify(
            x =>
                x.AddAppCultureAsync(
                    It.Is<CmsDataModels.AppCulture>(candidate => !ReferenceEquals(candidate, appCulture))
                ),
            Times.Once
        );
        appCultureBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
            x => x.Authorize((int?)appCulture.AppId, "AppCulture_create"),
            Times.Once
        );
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        AppCulture appCulture = CreateRandomAppCulture();


        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)appCulture.AppId, "AppCulture_create"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await appCultureService.AddAsync(appCulture);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        appCultureBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
            x => x.Authorize((int?)appCulture.AppId, "AppCulture_create"),
            Times.Once
        );
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

















