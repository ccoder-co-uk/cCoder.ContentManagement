// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.Exceptions;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AuthorizationProcessingServiceTests
{
    [Fact]
    public void UserWithRequestedPrivilege_WhenAuthorized_IsAllowed()
    {
        // Given
        const int appId = 7;
        const string userId = "test-user";
        User user = TestUsers.WithPrivilege(privilege: "page_read", appId: appId);

        Role[] roles = user.Roles.Select(selector: link => link.Role)
            .ToArray();

        AuthorizationContext context = new()
        {
            Request = new()
            {
                AppId = appId,
                Privilege = "page_read"
            }
        };

        authorizationServiceMock
            .Setup(expression: service => service.GetCurrentUserId())
            .Returns(value: userId);

        authorizationServiceMock
            .Setup(expression: service => service.GetRolesForUser(userId: userId))
            .Returns(value: new AuthorizationData { Roles = roles });

        authorizationServiceMock
            .Setup(expression: service => service.GetRolesForUser(userId: "Guest"))
            .Returns(value: new AuthorizationData { Roles = [] });

        authorizationServiceMock
            .Setup(expression: service => service.HasApps())
            .Returns(value: true);

        // When
        Action authorize = () => processingService
            .AuthorizeAuthorizationContext(authorizationContext: context);

        // Then
        authorize.Should()
            .NotThrow();

        authorizationServiceMock.VerifyAll();
    }

    [Fact]
    public void UserWithoutRequestedPrivilege_WhenAuthorized_IsDenied()
    {
        // Given
        AuthorizationContext context = new()
        {
            Request = new()
            {
                AppId = 7,
                Privilege = "page_read"
            }
        };

        authorizationServiceMock
            .Setup(expression: service => service.GetCurrentUserId())
            .Returns(value: "test-user");

        authorizationServiceMock
            .Setup(expression: service => service.GetRolesForUser(userId: "test-user"))
            .Returns(value: new AuthorizationData { Roles = [] });

        authorizationServiceMock
            .Setup(expression: service => service.GetRolesForUser(userId: "Guest"))
            .Returns(value: new AuthorizationData { Roles = [] });

        authorizationServiceMock
            .Setup(expression: service => service.HasApps())
            .Returns(value: true);

        // When
        Action authorize = () => processingService
            .AuthorizeAuthorizationContext(authorizationContext: context);

        // Then
        authorize.Should()
            .Throw<ContentManagementServiceException>()
            .Where(exceptionExpression: exception =>
                exception.InnerException is SecurityException
                && exception.InnerException.Message == "Access Denied!");

        authorizationServiceMock.VerifyAll();
    }

    [Fact]
    public void CurrentUser_WhenAuthorizationContextResolved_IsApplied()
    {
        // Given
        User user = new() { Id = "test-user" };
        AuthorizationContext context = new();

        authorizationServiceMock
            .Setup(expression: service => service.GetCurrentUser())
            .Returns(value: new AuthorizationData { User = user });

        authorizationServiceMock
            .Setup(expression: service => service.GetCurrentUserId())
            .Returns(value: user.Id);

        // When
        AuthorizationContext result = processingService
            .ResolveCurrentAuthorizationContext(authorizationContext: context);

        // Then
        result.Should()
            .BeSameAs(expected: context);

        result.User.Should()
            .BeSameAs(expected: user);

        result.UserId.Should()
            .Be(expected: user.Id);

        authorizationServiceMock.VerifyAll();
    }

    [Theory]
    [InlineData("en-GB", "fr-FR", "en-GB")]
    [InlineData(null, "fr-FR", "fr-FR")]
    public void RenderContext_WhenResolved_UsesRequestedOrDefaultCulture(
        string requestedCulture,
        string defaultCulture,
        string expectedCulture)
    {
        // Given
        User user = new()
        {
            Id = "test-user",
            DefaultCultureId = defaultCulture
        };

        AuthorizationContext context = new()
        {
            Culture = requestedCulture
        };

        authorizationServiceMock
            .Setup(expression: service => service.GetCurrentUser())
            .Returns(value: new AuthorizationData { User = user });

        authorizationServiceMock
            .Setup(expression: service => service.GetCurrentUserId())
            .Returns(value: user.Id);

        // When
        AuthorizationContext result = processingService
            .ResolveRenderAuthorizationContext(authorizationContext: context);

        // Then
        result.Should()
            .BeSameAs(expected: context);

        result.RenderAuthorization.Culture.Should()
            .Be(expected: expectedCulture);

        result.RenderAuthorization.User.Should()
            .BeSameAs(expected: user);

        authorizationServiceMock.VerifyAll();
    }
}