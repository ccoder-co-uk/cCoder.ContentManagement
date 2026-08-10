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
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class CultureServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        Culture culture = CreateRandomCulture(id: "en-GB");

        cultureBrokerMock.Setup(expression: x => x.GetAllCultures())
            .Returns(value: new[] { culture }.AsQueryable());

        appCultureBrokerMock.Setup(expression: x => x.GetAllAppCulturesIgnoringFilters())
            .Returns(value: new[] { new CmsDataModels.AppCulture { AppId = 7, CultureId = culture.Id } }.AsQueryable());

        authorizationManagerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Culture_delete"));

        cultureBrokerMock.Setup(expression: x => x.DeleteCultureAsync(
                It.Is<Culture>(deletedCulture =>
                    deletedCulture.Id == culture.Id &&
                    deletedCulture.Name == culture.Name)))
            .ReturnsAsync(value: 1);

        // When
        await cultureService.DeleteAsync(cultureId: "en-GB");

        // Then
        cultureBrokerMock.Verify(expression: x => x.GetAllCultures(), times: Times.Once);
        cultureBrokerMock.Verify(expression: x => x.DeleteCultureAsync(
            It.Is<Culture>(deletedCulture =>
                deletedCulture.Id == culture.Id &&
                deletedCulture.Name == culture.Name)), times: Times.Once);
        cultureBrokerMock.VerifyNoOtherCalls();
        appCultureBrokerMock.Verify(expression: x => x.GetAllAppCulturesIgnoringFilters(), times: Times.Once);
        appCultureBrokerMock.VerifyNoOtherCalls();
        authorizationManagerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Culture_delete"), times: Times.Once);
        authorizationManagerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Culture culture = CreateRandomCulture(id: "en-GB");

        cultureBrokerMock.Setup(expression: x => x.GetAllCultures())
            .Returns(value: new[] { culture }.AsQueryable());

        appCultureBrokerMock.Setup(expression: x => x.GetAllAppCulturesIgnoringFilters())
            .Returns(value: new[] { new CmsDataModels.AppCulture { AppId = 7, CultureId = culture.Id } }.AsQueryable());

        authorizationManagerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Culture_delete"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await cultureService.DeleteAsync(cultureId: "en-GB");

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        cultureBrokerMock.Verify(expression: x => x.GetAllCultures(), times: Times.Once);
        cultureBrokerMock.VerifyNoOtherCalls();
        appCultureBrokerMock.Verify(expression: x => x.GetAllAppCulturesIgnoringFilters(), times: Times.Once);
        appCultureBrokerMock.VerifyNoOtherCalls();
        authorizationManagerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Culture_delete"), times: Times.Once);
        authorizationManagerMock.VerifyNoOtherCalls();
    }

}