// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;
using cCoder.ContentManagement.Services.Processings.HttpContexts;
using cCoder.ContentManagement.Services.Processings.PageContexts;
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
        Mock<IHttpContextProcessingService> httpContextProcessingService = new();
        Mock<IPageAuthorizationProcessingService> pageAuthorizationProcessingService = new();
        Mock<IAuthorizationProcessingService> authorizationProcessingService = new();

        httpContextProcessingService.Setup(expression: service =>
            service.GetPageRenderContext())
            .Returns(value: context);

        pageAuthorizationProcessingService.Setup(expression: service =>
            service.AuthorizeHttpPageRenderContextAsync(context))
            .ReturnsAsync(value: context);

        authorizationProcessingService.Setup(expression: service =>
            service.ResolveCurrentAuthorizationContext(
                context: It.Is<AuthorizationContext>(match: item =>
                    item.Culture == context.Culture)))
            .Returns(value: new AuthorizationContext { User = user });

        PageContextOrchestrationService service = new(
            httpContextProcessingService: httpContextProcessingService.Object,
            pageAuthorizationProcessingService: pageAuthorizationProcessingService.Object,
            authorizationProcessingService: authorizationProcessingService.Object);

        // When
        HttpPageRenderContext result =
            await service.ResolvePageRenderContextAsync();

        // Then
        Assert.Same(expected: context, actual: result);
        Assert.Same(expected: user, actual: result.User);
        httpContextProcessingService.VerifyAll();
        pageAuthorizationProcessingService.VerifyAll();
        authorizationProcessingService.VerifyAll();
    }
}