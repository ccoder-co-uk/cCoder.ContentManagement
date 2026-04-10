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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        Culture culture = CreateRandomCulture();

        CmsDataModels.Culture submitted = null;

        appCultureBrokerMock.Setup(x => x.GetAllAppCultures(true)).Returns(new[] { new CmsDataModels.AppCulture { AppId = 7, CultureId = culture.Id } }.AsQueryable());
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "Culture_create"));

        cultureBrokerMock
            .Setup(x =>
                x.AddCultureAsync(It.Is<CmsDataModels.Culture>(candidate => !ReferenceEquals(candidate, culture)))
            )
            .Callback<CmsDataModels.Culture>(candidate => submitted = candidate)
            .ReturnsAsync((CmsDataModels.Culture value) => value);

        // When
        Culture result = await cultureService.AddAsync(culture);

        // Then
        result.Should().NotBeSameAs(culture);
        submitted.Should().NotBeNull();
        submitted!.Name.Should().Be(culture.Name);
        submitted.Id.Should().Be(culture.Id);
        submitted.Apps.Should().BeNull();
        submitted.MetaItems.Should().BeNull();
        submitted.PageContents.Should().BeNull();
        submitted.PageInfos.Should().BeNull();
        submitted.Users.Should().BeNull();
        result.Name.Should().Be(culture.Name);
        result.Id.Should().Be(culture.Id);
        result.Apps.Should().BeSameAs(culture.Apps);
        result.MetaItems.Should().BeSameAs(culture.MetaItems);
        result.PageContents.Should().BeSameAs(culture.PageContents);
        result.PageInfos.Should().BeSameAs(culture.PageInfos);
        result.Users.Should().BeSameAs(culture.Users);
        cultureBrokerMock.Verify(
            x =>
                x.AddCultureAsync(
                    It.Is<CmsDataModels.Culture>(candidate => !ReferenceEquals(candidate, culture))
                ),
            Times.Once
        );
        cultureBrokerMock.VerifyNoOtherCalls();
        appCultureBrokerMock.Verify(x => x.GetAllAppCultures(true), Times.Once);
        appCultureBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Culture_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        Culture culture = CreateRandomCulture();

        appCultureBrokerMock.Setup(x => x.GetAllAppCultures(true)).Returns(new[] { new CmsDataModels.AppCulture { AppId = 7, CultureId = culture.Id } }.AsQueryable());
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "Culture_create"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await cultureService.AddAsync(culture);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        cultureBrokerMock.VerifyNoOtherCalls();
        appCultureBrokerMock.Verify(x => x.GetAllAppCultures(true), Times.Once);
        appCultureBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Culture_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}















