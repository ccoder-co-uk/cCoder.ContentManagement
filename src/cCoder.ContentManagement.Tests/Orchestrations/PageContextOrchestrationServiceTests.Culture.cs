// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Foundations.Authorization;
using cCoder.ContentManagement.Services.Foundations.Authorizations;
using cCoder.ContentManagement.Services.Foundations.HttpContexts;
using cCoder.ContentManagement.Services.Orchestrations.PageContexts;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Orchestrations;

public sealed partial class PageContextOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldReplaceImplicitSessionCultureWithAuthenticatedUserDefaultAsync()
    {
        // Given
        HttpPageRenderContext context = new()
        {
            Culture = "fr-FR",
            CultureWasExplicitlyRequested = false
        };

        User user = new()
        {
            Id = "Paul",
            DefaultCultureId = "en-GB"
        };

        Mock<IHttpContextService> httpContextService = new();
        Mock<IPageAuthorizationService> pageAuthorizationService = new();
        Mock<IAuthorizationService> authorizationService = new();

        httpContextService.Setup(expression: service =>
            service.GetPageRenderContext())
            .Returns(value: context);

        pageAuthorizationService.Setup(expression: service =>
            service.AuthorizeHttpPageRenderContextAsync(
                pageRenderContext: context))
            .ReturnsAsync(value: context);

        authorizationService.Setup(expression: service =>
            service.ResolveCurrentAuthorizationContext(
                context: It.Is<AuthorizationContext>(match: item =>
                    item.Culture == "fr-FR")))
            .Returns(value: new AuthorizationContext { User = user });

        PageContextOrchestrationService service = new(
            httpContextService: httpContextService.Object,
            pageAuthorizationService: pageAuthorizationService.Object,
            authorizationService: authorizationService.Object);

        // When
        HttpPageRenderContext result =
            await service.ResolvePageRenderContextAsync();

        // Then
        Assert.Equal(expected: "en-GB", actual: result.Culture);
        httpContextService.VerifyAll();
        pageAuthorizationService.VerifyAll();
        authorizationService.VerifyAll();
    }

    [Fact]
    public async Task ShouldUseAuthenticatedUserDefaultCultureWhenSessionHasNoneAsync()
    {
        // Given
        HttpPageRenderContext context = new();

        User user = new()
        {
            Id = "Paul",
            DefaultCultureId = "fr-FR"
        };

        Mock<IHttpContextService> httpContextService = new();
        Mock<IPageAuthorizationService> pageAuthorizationService = new();
        Mock<IAuthorizationService> authorizationService = new();

        httpContextService.Setup(expression: service =>
            service.GetPageRenderContext())
            .Returns(value: context);

        pageAuthorizationService.Setup(expression: service =>
            service.AuthorizeHttpPageRenderContextAsync(
                pageRenderContext: context))
            .Callback(action: () => context.Culture = "en-GB")
            .ReturnsAsync(value: context);

        authorizationService.Setup(expression: service =>
            service.ResolveCurrentAuthorizationContext(
                context: It.Is<AuthorizationContext>(match: item =>
                    item.Culture == "en-GB")))
            .Returns(value: new AuthorizationContext { User = user });

        PageContextOrchestrationService service = new(
            httpContextService: httpContextService.Object,
            pageAuthorizationService: pageAuthorizationService.Object,
            authorizationService: authorizationService.Object);

        // When
        HttpPageRenderContext result =
            await service.ResolvePageRenderContextAsync();

        // Then
        Assert.Equal(expected: "fr-FR", actual: result.Culture);
        httpContextService.VerifyAll();
        pageAuthorizationService.VerifyAll();
        authorizationService.VerifyAll();
    }
}