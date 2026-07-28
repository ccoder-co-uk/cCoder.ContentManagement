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

public partial class AppServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        App app = CreateRandomApp(id: 5);

        CmsDataModels.App submitted = null;

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)app.Id, privilege: "App_update"));

        appBrokerMock
            .Setup(expression: x => x.UpdateAppAsync(updatedApp: It.IsAny<CmsDataModels.App>()))
            .Callback<CmsDataModels.App>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (CmsDataModels.App value) => value);

        // When
        App result = await appService.UpdateAppAsync(updatedApp: app);

        // Then

        result.Should()
            .BeSameAs(expected: app);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: app);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        result.Should()
            .BeEquivalentTo(expectation: new
            {
                app.Id,
                app.DefaultCultureId,
                app.TenantId,
                app.Name,
                app.Domain,
                app.DefaultTheme,
                app.ConfigJson
            });

        submitted.Should()
            .BeEquivalentTo(expectation: new
            {
                app.Id,
                app.DefaultCultureId,
                app.TenantId,
                app.Name,
                app.Domain,
                app.DefaultTheme,
                app.ConfigJson
            });

        submitted.Roles.Should()
            .BeNull();

        result.Roles.Should()
            .BeNull();

        appBrokerMock.Verify(expression: x => x.UpdateAppAsync(updatedApp: It.IsAny<CmsDataModels.App>()), times: Times.Once);
        appBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)app.Id, privilege: "App_update"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        App app = CreateRandomApp(id: 5);

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)app.Id, privilege: "App_update"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await appService.UpdateAppAsync(updatedApp: app);

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        appBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)app.Id, privilege: "App_update"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}