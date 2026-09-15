// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageRenderCacheOrchestrationServiceTests
{
    [Fact]
    public async Task AddPageRenderCacheAsync_WhenAuthorizationIsDenied_DoesNotPersistAsync()
    {
        // Given
        PageRenderCache cache = CreatePageRenderCache();
        DenyAuthorization();

        // When
        Func<Task> action = async () => await orchestrationService
            .AddPageRenderCacheAsync(newPageRenderCache: cache);

        // Then
        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        processingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ReplacePageRenderCachesAsync_WhenAuthorizationIsDenied_DoesNotPersistAsync()
    {
        // Given
        PageRenderCache cache = CreatePageRenderCache();
        DenyAuthorization();

        // When
        Func<Task> action = async () => await orchestrationService
            .ReplacePageRenderCachesAsync(
                appId: cache.AppId,
                pageIds: [cache.PageId],
                replacements: [cache]);

        // Then
        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        processingServiceMock.VerifyNoOtherCalls();
    }

    private void DenyAuthorization() =>
        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                context: It.IsAny<AuthorizationContext>()))
            .Throws(exception: new SecurityException(message: "Access Denied!"));
}