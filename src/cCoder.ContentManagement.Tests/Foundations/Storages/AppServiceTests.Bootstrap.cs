// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.ContentManagement.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class AppServiceTests
{
    [Fact]
    public void PrepareNewApp_WhenFirstApp_DefaultsThemeCulturesAndRoles()
    {
        // Given
        App app = new()
        {
            Name = "App",
            Domain = "app.local",
            DefaultTheme = string.Empty,
            Cultures = [],
            Roles = []
        };

        appBrokerMock.Setup(expression: broker => broker.GetCultures())
            .Returns(value: [new Culture { Id = string.Empty }]);

        appBrokerMock.Setup(expression: broker => broker.GetPrivileges())
            .Returns(value:
            [
                new Privilege
                {
                    Id = "app_create",
                    Operation = "Create",
                    Type = "App"
                },
                new Privilege
                {
                    Id = "app_read",
                    Operation = "Read",
                    Type = "App"
                }
            ]);

        appBrokerMock.Setup(expression: broker => broker.GetCurrentUser())
            .Returns(value: null);

        appBrokerMock.Setup(expression: broker => broker.GetCurrentUserId())
            .Returns(value: "admin");

        // When
        App result = appService.PrepareNewAppAppOperation(
            appOperation: new AppOperation
            {
                App = app,
                IsFirstApp = true
            })
        .App;

        // Then
        result.DefaultTheme.Should()
            .Be(expected: "Default");

        result.Cultures.Should()
            .ContainSingle();

        result.Roles.Select(selector: role => role.Name)
            .Should()
            .BeEquivalentTo(
                expectation: ["Administrators", "Users", "Guests", "System Admins"]);

        result.Roles.Single(predicate: role => role.Name == "System Admins")
            .Users.Should()
            .ContainSingle(predicate: userRole => userRole.UserId == "admin");

        appBrokerMock.VerifyAll();
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

        // When
        appService.StampAppChildrenAppOperation(
            appOperation: new AppOperation { App = app });

        // Then
        role.AppId.Should()
            .Be(expected: 42);

        role.App.Should()
            .BeNull();

        role.Users.Single().RoleId.Should()
            .Be(expected: role.Id);

        role.Users.Single().Role.Should()
            .BeNull();

        appBrokerMock.VerifyNoOtherCalls();
    }
}