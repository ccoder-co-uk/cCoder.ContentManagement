// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Foundations.Authorizations;
using cCoder.ContentManagement.Services.Foundations.HttpContexts;
using cCoder.ContentManagement.Services.Processings.HttpContexts;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Processings.PageContexts;

public sealed class HttpContextProcessingServiceTests
{
    [Fact]
    public void PageContext_WhenRequested_IsReturnedFromHttpContextService()
    {
        // Given
        HttpPageRenderContext context = new();
        Mock<IHttpContextService> httpContextService = new(MockBehavior.Strict);

        httpContextService
            .Setup(expression: service => service.GetPageRenderContext())
            .Returns(value: context);

        HttpContextProcessingService service = new(
            httpContextService: httpContextService.Object);

        // When
        HttpPageRenderContext result = service.GetPageRenderContext();

        // Then
        Assert.Same(expected: context, actual: result);
        httpContextService.VerifyAll();
    }
}