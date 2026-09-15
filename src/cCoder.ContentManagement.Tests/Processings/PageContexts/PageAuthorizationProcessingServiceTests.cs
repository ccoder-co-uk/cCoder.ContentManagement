// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Foundations.Authorizations;
using cCoder.ContentManagement.Services.Processings.PageContexts;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Processings.PageContexts;

public sealed partial class PageAuthorizationProcessingServiceTests
{
    [Fact]
    public async Task PageContext_WhenAuthorized_IsReturnedFromAuthorizationService()
    {
        // Given
        HttpPageRenderContext context = new();
        Mock<IPageAuthorizationService> pageAuthorizationService = new(MockBehavior.Strict);

        pageAuthorizationService
            .Setup(expression: service => service.AuthorizeHttpPageRenderContextAsync(pageRenderContext: context))
            .ReturnsAsync(value: context);

        PageAuthorizationProcessingService service = new(
            pageAuthorizationService: pageAuthorizationService.Object);

        // When
        HttpPageRenderContext result =
            await service.AuthorizeHttpPageRenderContextAsync(httpPageRenderContext: context);

        // Then
        Assert.Same(expected: context, actual: result);
        pageAuthorizationService.VerifyAll();
    }
}