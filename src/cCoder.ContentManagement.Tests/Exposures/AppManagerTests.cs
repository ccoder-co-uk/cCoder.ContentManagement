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

public sealed partial class AppManagerTests
{
    [Fact]
    public void GetUsers_WhenAppHasRoleUsers_ShouldReturnThoseUsers()
    {
        // Given
        const int appId = 23;
        User expectedUser = new() { Id = "user-id" };
        Mock<IAppManagerAggregationService> appManagerAggregationServiceMock = new();

        appManagerAggregationServiceMock.Setup(
            expression: service => service.GetUsersAppManagerContext(
                appManagerContext: It.Is<AppManagerContext>(
                    match: context => context.AppId == appId)))
            .Returns(value: new AppManagerContext
            {
                Users = new[] { expectedUser }.AsQueryable()
            });

        AppManager appManager = new(
            appManagerAggregationService: appManagerAggregationServiceMock.Object);

        // When
        User[] actualUsers = appManager.GetUsers(appId: appId)
            .ToArray();

        // Then
        actualUsers.Should()
            .Equal(elements: expectedUser);

        appManagerAggregationServiceMock.VerifyAll();
    }

    [Fact]
    public void GetUsers_WhenAppDoesNotExist_ShouldThrowSecurityException()
    {
        // Given
        const int appId = 23;
        Mock<IAppManagerAggregationService> appManagerAggregationServiceMock = new();

        appManagerAggregationServiceMock.Setup(
            expression: service => service.GetUsersAppManagerContext(
                appManagerContext: It.Is<AppManagerContext>(
                    match: context => context.AppId == appId)))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        AppManager appManager = new(
            appManagerAggregationService: appManagerAggregationServiceMock.Object);

        // When
        Action getUsers = () => appManager.GetUsers(appId: appId)
            .ToArray();

        // Then
        getUsers.Should()
            .Throw<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        appManagerAggregationServiceMock.VerifyAll();
    }
}