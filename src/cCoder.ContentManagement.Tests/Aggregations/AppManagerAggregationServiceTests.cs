// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Aggregations;

public sealed partial class AppManagerAggregationServiceTests
{
    [Fact]
    public void GetUsersAppManagerContext_WhenAppHasRoleUsers_ShouldReturnThoseUsers()
    {
        // Given
        const int appId = 23;
        User expectedUser = new() { Id = "user-id" };
        Mock<IAppOrchestrationService> appOrchestrationServiceMock = new();

        appOrchestrationServiceMock.Setup(
            expression: service => service.GetApp(appId: appId))
            .Returns(value: new App
            {
                Id = appId,
                Roles =
                [
                    new Role
                    {
                        Users =
                        [
                            new UserRole { User = expectedUser }
                        ]
                    }
                ]
            });

        AppManagerAggregationService service = new(
            appOrchestrationService: appOrchestrationServiceMock.Object);

        // When
        AppManagerContext result = service.GetUsersAppManagerContext(
            appManagerContext: new AppManagerContext { AppId = appId });

        // Then
        result.Users.Should()
            .Equal(elements: expectedUser);

        appOrchestrationServiceMock.VerifyAll();
    }

    [Fact]
    public void GetUsersAppManagerContext_WhenAppDoesNotExist_ShouldThrowSecurityException()
    {
        // Given
        const int appId = 23;
        Mock<IAppOrchestrationService> appOrchestrationServiceMock = new();

        appOrchestrationServiceMock.Setup(
            expression: service => service.GetApp(appId: appId))
            .Returns(value: (App)null);

        AppManagerAggregationService service = new(
            appOrchestrationService: appOrchestrationServiceMock.Object);

        // When
        Action getUsers = () => service.GetUsersAppManagerContext(
            appManagerContext: new AppManagerContext { AppId = appId });

        // Then
        getUsers.Should()
            .Throw<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        appOrchestrationServiceMock.VerifyAll();
    }
}