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

public sealed class AppManagerAggregationServiceTests
{
    [Fact]
    public void GetUsersAppManagerContext_WhenAppHasRoleUsers_ShouldReturnThoseUsers()
    {
        const int appId = 23;
        User expectedUser = new() { Id = "user-id" };
        Mock<IAppOrchestrationService> appOrchestrationServiceMock = new();

        appOrchestrationServiceMock.Setup(service => service.GetApp(appId))
            .Returns(new App
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

        AppManagerContext result = service.GetUsersAppManagerContext(
            appManagerContext: new AppManagerContext { AppId = appId });

        result.Users.Should().Equal(expectedUser);
        appOrchestrationServiceMock.VerifyAll();
    }

    [Fact]
    public void GetUsersAppManagerContext_WhenAppDoesNotExist_ShouldThrowSecurityException()
    {
        const int appId = 23;
        Mock<IAppOrchestrationService> appOrchestrationServiceMock = new();
        appOrchestrationServiceMock.Setup(service => service.GetApp(appId))
            .Returns((App)null);
        AppManagerAggregationService service = new(
            appOrchestrationService: appOrchestrationServiceMock.Object);

        Action getUsers = () => service.GetUsersAppManagerContext(
            appManagerContext: new AppManagerContext { AppId = appId });

        getUsers.Should().Throw<SecurityException>()
            .WithMessage("Access Denied!");
        appOrchestrationServiceMock.VerifyAll();
    }
}