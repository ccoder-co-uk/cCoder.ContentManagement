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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        App app = CreateRandomApp(id: 0);
        Guid roleId = Guid.NewGuid();

        app.Roles =
        [
            new Role
            {
                Id = roleId,
                Name = "Administrators",
                Privs = "app_admin",
                Users = [new UserRole { RoleId = roleId, UserId = "paul" }],
            },
        ];

        CmsDataModels.App submitted = null;

        appBrokerMock
            .Setup(expression: x => x.GetAllAppsIgnoringFilters())
            .Returns(value: new[] { CreateRandomApp() }.AsQueryable());

        authorizationManagerMock.Setup(expression: x => x.Authorize(appId: It.Is<int?>(match: appId => appId == null), privilege: "App_create"));

        appBrokerMock
            .Setup(expression: x => x.AddAppAsync(newApp: It.IsAny<CmsDataModels.App>()))
            .Callback<CmsDataModels.App>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (CmsDataModels.App value) => value);

        // When
        App result = await appService.AddAppAsync(newApp: app);

        // Then

        result.Should()
            .BeSameAs(expected: app);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: app);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

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

        result.Roles.Should()
            .BeEquivalentTo(expectation: app.Roles);

        appBrokerMock.Verify(
expression: x => x.AddAppAsync(newApp: It.IsAny<CmsDataModels.App>()),
times: Times.Once
        );

        appBrokerMock.Verify(
            expression: x => x.GetAllAppsIgnoringFilters(),
            times: Times.Once);

        appBrokerMock.VerifyNoOtherCalls();
        authorizationManagerMock.Verify(expression: x => x.Authorize(appId: It.Is<int?>(match: appId => appId == null), privilege: "App_create"), times: Times.Once);
        authorizationManagerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        App app = CreateRandomApp(id: 0);

        appBrokerMock
            .Setup(expression: x => x.GetAllAppsIgnoringFilters())
            .Returns(value: new[] { CreateRandomApp() }.AsQueryable());

        authorizationManagerMock
            .Setup(expression: x => x.Authorize(appId: It.Is<int?>(match: appId => appId == null), privilege: "App_create"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await appService.AddAppAsync(newApp: app);

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        appBrokerMock.Verify(
            expression: x => x.GetAllAppsIgnoringFilters(),
            times: Times.Once);

        appBrokerMock.VerifyNoOtherCalls();
        authorizationManagerMock.Verify(expression: x => x.Authorize(appId: It.Is<int?>(match: appId => appId == null), privilege: "App_create"), times: Times.Once);
        authorizationManagerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldAllowFirstAppWithoutExistingAppPrivilegeForAddAsync()
    {
        // Given
        App app = CreateRandomApp(id: 0);

        appBrokerMock
            .Setup(expression: x => x.GetAllAppsIgnoringFilters())
            .Returns(
                value: Array.Empty<App>()
                    .AsQueryable());

        appBrokerMock
            .Setup(expression: x => x.AddAppAsync(newApp: It.IsAny<App>()))
            .ReturnsAsync(value: app);

        // When
        App result = await appService.AddAppAsync(newApp: app);

        // Then
        result.Should()
            .BeSameAs(expected: app);

        appBrokerMock.VerifyAll();
        authorizationManagerMock.VerifyNoOtherCalls();
    }

}