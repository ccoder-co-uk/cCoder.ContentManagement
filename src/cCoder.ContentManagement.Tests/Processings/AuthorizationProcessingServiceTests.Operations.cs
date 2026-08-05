// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AuthorizationProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateAuthorizationOperations()
    {
        // Given
        AuthorizationContext context = new();
        AuthorizationContext resolvedContext = new();

        authorizationServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                context: context));

        authorizationServiceMock
            .Setup(expression: service => service.ResolveCurrentAuthorizationContext(
                context: context))
            .Returns(value: resolvedContext);

        authorizationServiceMock
            .Setup(expression: service => service.IsAdminAuthorizationContext(
                context: context))
            .Returns(value: true);

        authorizationServiceMock
            .Setup(expression: service => service.IsAdminOfAppAuthorizationContext(
                context: context))
            .Returns(value: true);

        authorizationServiceMock
            .Setup(expression: service => service.UserCanPageAuthorizationContext(
                context: context))
            .Returns(value: true);

        // When
        processingService.AuthorizeAuthorizationContext(context: context);

        AuthorizationContext actualContext =
            processingService.ResolveCurrentAuthorizationContext(
                context: context);

        bool isAdmin = processingService.IsAdminAuthorizationContext(
            context: context);

        bool isAppAdmin = processingService
            .IsAdminOfAppAuthorizationContext(context: context);

        bool canAccessPage = processingService
            .UserCanPageAuthorizationContext(context: context);

        // Then
        actualContext.Should()
            .BeSameAs(expected: resolvedContext);

        isAdmin.Should()
            .BeTrue();

        isAppAdmin.Should()
            .BeTrue();

        canAccessPage.Should()
            .BeTrue();

        authorizationServiceMock.VerifyAll();
    }

    [Theory]
    [InlineData("en-GB", "fr-FR", "en-GB")]
    [InlineData(null, "fr-FR", "fr-FR")]
    public void ShouldResolveRenderAuthorizationCulture(
        string requestedCulture,
        string defaultCulture,
        string expectedCulture)
    {
        // Given
        User user = new()
        {
            DefaultCultureId = defaultCulture
        };

        AuthorizationContext context = new();

        AuthorizationContext resolvedContext = new()
        {
            Culture = requestedCulture,
            User = user
        };

        authorizationServiceMock
            .Setup(expression: service => service.ResolveCurrentAuthorizationContext(
                context: context))
            .Returns(value: resolvedContext);

        // When
        AuthorizationContext result = processingService
            .ResolveRenderAuthorizationContext(context: context);

        // Then
        result.Should()
            .BeSameAs(expected: resolvedContext);

        result.RenderAuthorization.Culture.Should()
            .Be(expected: expectedCulture);

        result.RenderAuthorization.User.Should()
            .BeSameAs(expected: user);
    }
}