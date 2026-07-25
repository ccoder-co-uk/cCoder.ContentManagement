// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
using SecurityDataModels = cCoder.Data.Models.Security;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AppProcessingServiceTests
{
    [Fact]
    public async Task ShouldDefaultThemeAndReturnAddedAppWhenUserCanCreateForAddAsync()
    {
        // Given
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                {
                    throw new SecurityException(message: "Access Denied!");
                }
            });

        authorizationBrokerMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        User actor = TestUsers.WithPrivilege(privilege: "app_create");
        App inputApp = CreateRandomApp();
        inputApp.DefaultTheme = string.Empty;
        inputApp.Cultures = [];

        inputApp.Roles =
        [
            new Role
            {
                Id = Guid.NewGuid(),
                AppId = inputApp.Id,
                Name = "Administrators",
                Privs = "app_admin,app_delete"
            }
        ];

        currentUser = actor;

        appServiceMock
            .Setup(expression: x => x.GetAllApp(ignoreFilters: true))
            .Returns(value: Array.Empty<App>()
            .AsQueryable());

        appServiceMock.Setup(expression: x => x.AddAppAsync(newApp: It.IsAny<App>()))
            .ReturnsAsync(valueFunction: (App candidate) =>
            {
                candidate.Id = 1;
                return candidate;
            });

        cultureBrokerMock
            .Setup(expression: x => x.GetAllCultures(ignoreFilters: false))
            .Returns(value: new[] { new Culture { Id = string.Empty } }.AsQueryable());

        privilegeBrokerMock
            .Setup(expression: x => x.GetAllPrivileges(ignoreFilters: false))
            .Returns(
value: new[]
                {
                    new SecurityDataModels.Privilege { Id = "app_create", Operation = "Create", Type = "App" },
                    new SecurityDataModels.Privilege { Id = "app_read", Operation = "Read", Type = "App" },
                    new SecurityDataModels.Privilege { Id = "app_update", Operation = "Update", Type = "App" },
                    new SecurityDataModels.Privilege { Id = "app_delete", Operation = "Delete", Type = "App" }
                }.AsQueryable());

        // When
        App result = await appProcessingService.AddAppAsync(newApp: inputApp);

        // Then
        result.Id.Should()
            .Be(expected: 1);

        result.DefaultTheme.Should()
            .Be(expected: "Default");

        result.Cultures.Should()
            .HaveCount(expected: 1);

        result.Roles.Should()
            .HaveCount(expected: 4);

        appServiceMock.Verify(
expression: x =>
                    x.AddAppAsync(
newApp: It.Is<App>(match: app =>
                            app.DefaultTheme == "Default"
                            && app.Cultures.Count == 1
                            && app.Roles.Count == 4
                            && app.Roles.Any(predicate: role => role.Name == "Administrators")
                            && app.Roles.Any(predicate: role => role.Name == "Users")
                            && app.Roles.Any(predicate: role => role.Name == "Guests")
                            && app.Roles.Any(predicate: role => role.Name == "System Admins")
                        )
                    ),
times: Times.Once
        );

        cultureBrokerMock.Verify(expression: x => x.GetAllCultures(ignoreFilters: false), times: Times.Once);
        privilegeBrokerMock.Verify(expression: x => x.GetAllPrivileges(ignoreFilters: false), times: Times.Exactly(callCount: 2));
        appServiceMock.Verify(expression: x => x.GetAllApp(ignoreFilters: true), times: Times.Once);
        appServiceMock.VerifyNoOtherCalls();
        cultureBrokerMock.VerifyNoOtherCalls();
        privilegeBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenAddAsync()
    {
        // Given
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                {
                    throw new SecurityException(message: "Access Denied!");
                }
            });

        authorizationBrokerMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        App app = new() { Name = "App", Domain = "app.local" };

        currentUser = TestUsers.WithoutPrivileges();

        appServiceMock
            .Setup(expression: x => x.GetAllApp(ignoreFilters: true))
            .Returns(value: new[] { new App { Id = 99, Domain = "existing.local" } }.AsQueryable());

        cultureBrokerMock
            .Setup(expression: x => x.GetAllCultures(ignoreFilters: false))
            .Returns(value: new[] { new Culture { Id = string.Empty } }.AsQueryable());

        privilegeBrokerMock
            .Setup(expression: x => x.GetAllPrivileges(ignoreFilters: false))
            .Returns(value: Array.Empty<SecurityDataModels.Privilege>()
            .AsQueryable());

        appServiceMock
            .Setup(expression: x => x.AddAppAsync(newApp: It.IsAny<App>()))
            .ThrowsAsync(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> act = async () => await appProcessingService.AddAppAsync(newApp: app);

        // Then
        await act.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        cultureBrokerMock.Verify(expression: x => x.GetAllCultures(ignoreFilters: false), times: Times.Once);
        privilegeBrokerMock.Verify(expression: x => x.GetAllPrivileges(ignoreFilters: false), times: Times.Exactly(callCount: 2));
        appServiceMock.Verify(expression: x => x.GetAllApp(ignoreFilters: true), times: Times.Once);
        appServiceMock.Verify(expression: x => x.AddAppAsync(newApp: It.IsAny<App>()), times: Times.Once);
        appServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldGrantAppCreateToAuthenticatedBootstrapUserWhenCreatingFirstApp()
    {
        // Given
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => null);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUserId())
            .Returns(value: "admin");

        appServiceMock
            .Setup(expression: x => x.GetAllApp(ignoreFilters: true))
            .Returns(value: Array.Empty<App>()
            .AsQueryable());

        appServiceMock
            .Setup(expression: x => x.AddAppAsync(newApp: It.IsAny<App>()))
            .ReturnsAsync(valueFunction: (App candidate) =>
            {
                candidate.Id = 1;
                return candidate;
            });

        cultureBrokerMock
            .Setup(expression: x => x.GetAllCultures(ignoreFilters: false))
            .Returns(value: new[] { new Culture { Id = string.Empty } }.AsQueryable());

        privilegeBrokerMock
            .Setup(expression: x => x.GetAllPrivileges(ignoreFilters: false))
            .Returns(
value: new[]
                {
                    new SecurityDataModels.Privilege { Id = "app_create", Operation = "Create", Type = "App" },
                    new SecurityDataModels.Privilege { Id = "app_read", Operation = "Read", Type = "App" },
                    new SecurityDataModels.Privilege { Id = "app_admin", Operation = "Admin", Type = "App" }
                }.AsQueryable());

        // When
        App result = await appProcessingService.AddAppAsync(newApp: CreateRandomApp());

        Role administrators = result.Roles.Single(predicate: role => role.Name == "Administrators");
        Role users = result.Roles.Single(predicate: role => role.Name == "Users");
        Role guests = result.Roles.Single(predicate: role => role.Name == "Guests");
        Role systemAdmins = result.Roles.Single(predicate: role => role.Name == "System Admins");

        // Then
        administrators.Privileges.Should()
            .Contain(expected: "app_create");

        administrators.Users.Should()
            .ContainSingle(predicate: userRole =>
                userRole.UserId == "admin");

        users.Users.Should()
            .ContainSingle(predicate: userRole =>
                userRole.UserId == "admin");

        guests.Users.Should()
            .ContainSingle(predicate: userRole => userRole.UserId == "Guest");

        systemAdmins.Privileges.Should()
            .Contain(expected: "app_create");

        systemAdmins.Users.Should()
            .ContainSingle(predicate: userRole =>
                userRole.UserId == "admin");
    }

    [Fact]
    public async Task ShouldReturnAddedAppWithoutRoleBackReferencesForAddAsync()
    {
        // Given
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => TestUsers.WithPrivilege(privilege: "app_create"));

        appServiceMock
            .Setup(expression: x => x.GetAllApp(ignoreFilters: true))
            .Returns(value: Array.Empty<App>()
            .AsQueryable());

        appServiceMock
            .Setup(expression: x => x.AddAppAsync(newApp: It.IsAny<App>()))
            .ReturnsAsync(valueFunction: (App candidate) =>
            {
                candidate.Id = 1;
                return candidate;
            });

        cultureBrokerMock
            .Setup(expression: x => x.GetAllCultures(ignoreFilters: false))
            .Returns(value: new[] { new Culture { Id = string.Empty } }.AsQueryable());

        privilegeBrokerMock
            .Setup(expression: x => x.GetAllPrivileges(ignoreFilters: false))
            .Returns(
value: new[]
                {
                    new SecurityDataModels.Privilege { Id = "app_create", Operation = "Create", Type = "App" },
                    new SecurityDataModels.Privilege { Id = "app_read", Operation = "Read", Type = "App" },
                    new SecurityDataModels.Privilege { Id = "app_admin", Operation = "Admin", Type = "App" }
                }.AsQueryable());

        // When
        App result = await appProcessingService.AddAppAsync(newApp: CreateRandomApp());

        // Then
        result.Roles.Should()
            .NotBeEmpty();

        result.Roles.Should()
            .OnlyContain(predicate: role => role.App == null);

        result.Roles.SelectMany(selector: role => role.Users ?? Array.Empty<UserRole>())
            .Should()
            .OnlyContain(predicate: userRole => userRole.Role == null);
    }

}