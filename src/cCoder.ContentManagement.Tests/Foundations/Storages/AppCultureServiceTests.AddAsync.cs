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

public partial class AppCultureServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        AppCulture appCulture = CreateRandomAppCulture();

        CmsDataModels.AppCulture submitted = null;

        authorizationBrokerMock.Setup(expression: x =>
            x.Authorize(appId: (int?)appCulture.AppId, privilege: "AppCulture_create")
        );

        appCultureBrokerMock
            .Setup(expression: x =>
                x.AddAppCultureAsync(
newAppCulture: It.Is<CmsDataModels.AppCulture>(match: candidate => !ReferenceEquals(objA: candidate, objB: appCulture))
                )
            )
            .Callback<CmsDataModels.AppCulture>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (CmsDataModels.AppCulture value) => value);

        // When
        AppCulture result = await appCultureService.AddAppCultureAsync(newAppCulture: appCulture);

        // Then

        result.Should()
            .BeSameAs(expected: appCulture);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: appCulture);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted.Should()
            .BeEquivalentTo(expectation: appCulture);

        result.Should()
            .BeEquivalentTo(expectation: appCulture);

        appCultureBrokerMock.Verify(
expression: x =>
                x.AddAppCultureAsync(
newAppCulture: It.Is<CmsDataModels.AppCulture>(match: candidate => !ReferenceEquals(objA: candidate, objB: appCulture))
                ),
times: Times.Once
        );

        appCultureBrokerMock.VerifyNoOtherCalls();

        authorizationBrokerMock.Verify(
expression: x => x.Authorize(appId: (int?)appCulture.AppId, privilege: "AppCulture_create"),
times: Times.Once
        );

        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        AppCulture appCulture = CreateRandomAppCulture();


        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)appCulture.AppId, privilege: "AppCulture_create"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await appCultureService.AddAppCultureAsync(newAppCulture: appCulture);

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        appCultureBrokerMock.VerifyNoOtherCalls();

        authorizationBrokerMock.Verify(
expression: x => x.Authorize(appId: (int?)appCulture.AppId, privilege: "AppCulture_create"),
times: Times.Once
        );

        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}