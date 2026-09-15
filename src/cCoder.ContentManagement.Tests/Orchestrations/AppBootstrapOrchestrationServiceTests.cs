// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Foundations.Authorization;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Orchestrations;

public sealed partial class AppBootstrapOrchestrationServiceTests
{
    [Fact]
    public void PrepareNewApp_WhenFirstApp_DefaultsThemeCulturesAndRoles()
    {
        // Given
        Mock<ICultureService> cultureService = new(MockBehavior.Strict);
        Mock<IPrivilegeService> privilegeService = new(MockBehavior.Strict);
        Mock<IAuthorizationService> authorizationService = new(MockBehavior.Strict);

        App app = new()
        {
            Name = "App",
            Domain = "app.local",
            DefaultTheme = string.Empty,
            Cultures = [],
            Roles = []
        };

        cultureService.Setup(expression: service => service.GetAllCulture(ignoreFilters: false))
            .Returns(value: new[] { new Culture { Id = string.Empty } }.AsQueryable());

        privilegeService.Setup(expression: service => service.GetAllPrivileges())
            .Returns(value: new[]
            {
                new Privilege { Id = "app_create", Operation = "Create", Type = "App" },
                new Privilege { Id = "app_read", Operation = "Read", Type = "App" }
            }.AsQueryable());

        authorizationService.Setup(expression: service => service.GetCurrentUser())
            .Returns(value: new AuthorizationData());

        authorizationService.Setup(expression: service => service.GetCurrentUserId())
            .Returns(value: "admin");

        AppBootstrapOrchestrationService service = new(
            cultureService: cultureService.Object,
            privilegeService: privilegeService.Object,
            authorizationService: authorizationService.Object);

        // When
        App result = service.PrepareNewApp(app: app, isFirstApp: true);

        // Then
        result.DefaultTheme.Should()
            .Be(expected: "Default");

        result.Cultures.Should()
            .ContainSingle();

        result.Roles.Select(selector: role => role.Name)
            .Should()
            .BeEquivalentTo(expectation: ["Administrators", "Users", "Guests", "System Admins"]);

        result.Roles.Single(predicate: role => role.Name == "System Admins")
            .Users.Should()
            .ContainSingle(predicate: userRole => userRole.UserId == "admin");

        cultureService.VerifyAll();
        privilegeService.VerifyAll();
        authorizationService.VerifyAll();
    }

    [Fact]
    public void StampAppChildren_WhenCalled_RemovesRoleBackReferences()
    {
        // Given
        Role role = new()
        {
            Id = Guid.NewGuid(),
            App = new App(),
            Users = [new UserRole { Role = new Role() }]
        };

        App app = new() { Id = 42, Roles = [role] };

        AppBootstrapOrchestrationService service = new(
            cultureService: Mock.Of<ICultureService>(),
            privilegeService: Mock.Of<IPrivilegeService>(),
            authorizationService: Mock.Of<IAuthorizationService>());

        // When
        service.StampAppChildren(app: app);

        // Then
        role.AppId.Should()
            .Be(expected: 42);

        role.App.Should()
            .BeNull();

        role.Users.Single().RoleId.Should()
            .Be(expected: role.Id);

        role.Users.Single().Role.Should()
            .BeNull();
    }
}