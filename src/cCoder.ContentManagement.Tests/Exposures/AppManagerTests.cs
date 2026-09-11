// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using System.Security;
using Xunit;

namespace cCoder.ContentManagement.Tests.Exposures;

public sealed class AppManagerTests
{
    [Fact]
    public void GetUsers_WhenAppHasRoleUsers_ShouldReturnThoseUsers()
    {
        const int appId = 23;
        User expectedUser = new() { Id = "user-id" };
        Mock<IAppManagerAggregationService> appManagerAggregationServiceMock = new();

        appManagerAggregationServiceMock.Setup(service => service.GetUsersAppManagerContext(
                It.Is<AppManagerContext>(context => context.AppId == appId)))
            .Returns(new AppManagerContext
            {
                Users = new[] { expectedUser }.AsQueryable()
            });

        AppManager appManager = new(
            appManagerAggregationService: appManagerAggregationServiceMock.Object);

        User[] actualUsers = appManager.GetUsers(appId: appId).ToArray();

        actualUsers.Should().Equal(expectedUser);
        appManagerAggregationServiceMock.VerifyAll();
    }

    [Fact]
    public void GetUsers_WhenAppDoesNotExist_ShouldThrowSecurityException()
    {
        const int appId = 23;
        Mock<IAppManagerAggregationService> appManagerAggregationServiceMock = new();

        appManagerAggregationServiceMock.Setup(service => service.GetUsersAppManagerContext(
                It.Is<AppManagerContext>(context => context.AppId == appId)))
            .Throws(new SecurityException(message: "Access Denied!"));

        AppManager appManager = new(
            appManagerAggregationService: appManagerAggregationServiceMock.Object);

        Action getUsers = () => appManager.GetUsers(appId: appId).ToArray();

        getUsers.Should().Throw<SecurityException>()
            .WithMessage("Access Denied!");
        appManagerAggregationServiceMock.VerifyAll();
    }
}