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
    public async Task ShouldResolveAuthorizationAndCurrentUserAsync()
    {
        // Given
        HttpPageRenderContext context = new() { Culture = "en-GB" };
        User user = new() { Id = "Paul" };
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
                    item.Culture == context.Culture)))
            .Returns(value: new AuthorizationContext { User = user });

        PageContextOrchestrationService service = new(
            httpContextService: httpContextService.Object,
            pageAuthorizationService: pageAuthorizationService.Object,
            authorizationService: authorizationService.Object);

        // When
        HttpPageRenderContext result =
            await service.ResolvePageRenderContextAsync();

        // Then
        Assert.Same(expected: context, actual: result);
        Assert.Same(expected: user, actual: result.User);
        httpContextService.VerifyAll();
        pageAuthorizationService.VerifyAll();
        authorizationService.VerifyAll();
    }
}