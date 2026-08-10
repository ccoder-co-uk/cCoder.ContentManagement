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

#pragma warning disable STXFORMAT005, STXFORMAT008


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class CultureServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        Culture culture = CreateRandomCulture();

        CmsDataModels.Culture submitted = null;

        appCultureBrokerMock.Setup(expression: x => x.GetAllAppCulturesIgnoringFilters())
            .Returns(value: new[] { new CmsDataModels.AppCulture { AppId = 7, CultureId = culture.Id } }
                .AsQueryable());

        authorizationManagerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Culture_create"));

        cultureBrokerMock.Setup(expression: x => x.AddCultureAsync(newCulture: It.IsAny<CmsDataModels.Culture>()))
            .Callback<CmsDataModels.Culture>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (CmsDataModels.Culture value) => value);

        // When
        Culture result = await cultureService.AddCultureAsync(newCulture: culture);

        // Then

        Assert.Same(expected: culture, actual: result);
        Assert.NotNull(@object: submitted);
        Assert.NotSame(expected: culture, actual: submitted);
        Assert.NotSame(expected: submitted, actual: result);
        Assert.Equal(expected: culture.Name, actual: submitted.Name);
        Assert.Equal(expected: culture.Id, actual: submitted.Id);
        Assert.Null(@object: submitted.Apps);
        Assert.Null(@object: submitted.MetaItems);
        Assert.Null(@object: submitted.PageContents);
        Assert.Null(@object: submitted.PageInfos);
        Assert.Null(@object: submitted.Users);
        Assert.Equal(expected: culture.Name, actual: result.Name);
        Assert.Equal(expected: culture.Id, actual: result.Id);
        Assert.Same(expected: culture.Apps, actual: result.Apps);
        Assert.Same(expected: culture.MetaItems, actual: result.MetaItems);
        Assert.Same(expected: culture.PageContents, actual: result.PageContents);
        Assert.Same(expected: culture.PageInfos, actual: result.PageInfos);
        Assert.Same(expected: culture.Users, actual: result.Users);

        cultureBrokerMock.Verify(expression: x => x.AddCultureAsync(
            newCulture: It.IsAny<CmsDataModels.Culture>()), times: Times.Once);

        cultureBrokerMock.VerifyNoOtherCalls();
        appCultureBrokerMock.Verify(expression: x => x.GetAllAppCulturesIgnoringFilters(), times: Times.Once);
        appCultureBrokerMock.VerifyNoOtherCalls();
        authorizationManagerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Culture_create"), times: Times.Once);
        authorizationManagerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        Culture culture = CreateRandomCulture();

        appCultureBrokerMock.Setup(expression: x => x.GetAllAppCulturesIgnoringFilters())
            .Returns(value: new[] { new CmsDataModels.AppCulture { AppId = 7, CultureId = culture.Id } }.AsQueryable());

        authorizationManagerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Culture_create"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await cultureService.AddCultureAsync(newCulture: culture);

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        cultureBrokerMock.VerifyNoOtherCalls();
        appCultureBrokerMock.Verify(expression: x => x.GetAllAppCulturesIgnoringFilters(), times: Times.Once);
        appCultureBrokerMock.VerifyNoOtherCalls();
        authorizationManagerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Culture_create"), times: Times.Once);
        authorizationManagerMock.VerifyNoOtherCalls();
    }

}