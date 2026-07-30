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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        Culture culture = CreateRandomCulture();

        CmsDataModels.Culture submitted = null;

        appCultureBrokerMock.Setup(expression: x => x.GetAllAppCulturesIgnoringFilters())
            .Returns(value: new[] { new CmsDataModels.AppCulture { AppId = 7, CultureId = culture.Id } }.AsQueryable());

        authorizationManagerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Culture_create"));

        cultureBrokerMock
            .Setup(expression: x =>
                x.AddCultureAsync(newCulture: It.Is<CmsDataModels.Culture>(match: candidate => !ReferenceEquals(objA: candidate, objB: culture)))
            )
            .Callback<CmsDataModels.Culture>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (CmsDataModels.Culture value) => value);

        // When
        Culture result = await cultureService.AddCultureAsync(newCulture: culture);

        // Then

        result.Should()
            .BeSameAs(expected: culture);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: culture);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted!.Name.Should()
            .Be(expected: culture.Name);

        submitted.Id.Should()
            .Be(expected: culture.Id);

        submitted.Apps.Should()
            .BeNull();

        submitted.MetaItems.Should()
            .BeNull();

        submitted.PageContents.Should()
            .BeNull();

        submitted.PageInfos.Should()
            .BeNull();

        submitted.Users.Should()
            .BeNull();

        result.Name.Should()
            .Be(expected: culture.Name);

        result.Id.Should()
            .Be(expected: culture.Id);

        result.Apps.Should()
            .BeSameAs(expected: culture.Apps);

        result.MetaItems.Should()
            .BeSameAs(expected: culture.MetaItems);

        result.PageContents.Should()
            .BeSameAs(expected: culture.PageContents);

        result.PageInfos.Should()
            .BeSameAs(expected: culture.PageInfos);

        result.Users.Should()
            .BeSameAs(expected: culture.Users);

        cultureBrokerMock.Verify(
expression: x =>
                x.AddCultureAsync(
newCulture: It.Is<CmsDataModels.Culture>(match: candidate => !ReferenceEquals(objA: candidate, objB: culture))
                ),
times: Times.Once
        );

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