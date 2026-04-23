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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        Culture culture = CreateRandomCulture();

        CmsDataModels.Culture submitted = null;

        appCultureBrokerMock.Setup(x => x.GetAllAppCultures(true)).Returns(new[] { new CmsDataModels.AppCulture { AppId = 7, CultureId = culture.Id } }.AsQueryable());
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "Culture_update"));

        cultureBrokerMock
            .Setup(x => x.UpdateCultureAsync(It.IsAny<CmsDataModels.Culture>()))
            .Callback<CmsDataModels.Culture>(candidate => submitted = candidate)
            .ReturnsAsync((CmsDataModels.Culture value) => value);

        // When
        Culture result = await cultureService.UpdateAsync(culture);

        // Then
        result.Should().BeSameAs(culture);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(culture);
        result.Should().NotBeSameAs(submitted);
        submitted.Should().BeEquivalentTo(culture);
        result.Should().BeEquivalentTo(culture);
        cultureBrokerMock.Verify(x => x.UpdateCultureAsync(It.IsAny<CmsDataModels.Culture>()), Times.Once);
        cultureBrokerMock.VerifyNoOtherCalls();
        appCultureBrokerMock.Verify(x => x.GetAllAppCultures(true), Times.Once);
        appCultureBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Culture_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        Culture culture = CreateRandomCulture();

        appCultureBrokerMock.Setup(x => x.GetAllAppCultures(true)).Returns(new[] { new CmsDataModels.AppCulture { AppId = 7, CultureId = culture.Id } }.AsQueryable());
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "Culture_update"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await cultureService.UpdateAsync(culture);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        cultureBrokerMock.VerifyNoOtherCalls();
        appCultureBrokerMock.Verify(x => x.GetAllAppCultures(true), Times.Once);
        appCultureBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Culture_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}















