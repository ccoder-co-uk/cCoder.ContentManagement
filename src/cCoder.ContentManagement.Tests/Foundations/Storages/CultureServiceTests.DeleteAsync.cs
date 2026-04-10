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

public partial class CultureServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        Culture culture = CreateRandomCulture("en-GB");

        cultureBrokerMock.Setup(x => x.GetAllCultures(false)).Returns(new[] { culture }.AsQueryable());

        appCultureBrokerMock.Setup(x => x.GetAllAppCultures(true)).Returns(new[] { new CmsDataModels.AppCulture { AppId = 7, CultureId = culture.Id } }.AsQueryable());
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "Culture_delete"));
        cultureBrokerMock.Setup(x => x.DeleteCultureAsync(culture)).ReturnsAsync(1);

        // When
        await cultureService.DeleteAsync("en-GB");

        // Then
        cultureBrokerMock.Verify(x => x.GetAllCultures(false), Times.Once);
        cultureBrokerMock.Verify(x => x.DeleteCultureAsync(culture), Times.Once);
        cultureBrokerMock.VerifyNoOtherCalls();
        appCultureBrokerMock.Verify(x => x.GetAllAppCultures(true), Times.Once);
        appCultureBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Culture_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Culture culture = CreateRandomCulture("en-GB");

        cultureBrokerMock.Setup(x => x.GetAllCultures(false)).Returns(new[] { culture }.AsQueryable());

        appCultureBrokerMock.Setup(x => x.GetAllAppCultures(true)).Returns(new[] { new CmsDataModels.AppCulture { AppId = 7, CultureId = culture.Id } }.AsQueryable());
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "Culture_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await cultureService.DeleteAsync("en-GB");

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        cultureBrokerMock.Verify(x => x.GetAllCultures(false), Times.Once);
        cultureBrokerMock.VerifyNoOtherCalls();
        appCultureBrokerMock.Verify(x => x.GetAllAppCultures(true), Times.Once);
        appCultureBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Culture_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}















