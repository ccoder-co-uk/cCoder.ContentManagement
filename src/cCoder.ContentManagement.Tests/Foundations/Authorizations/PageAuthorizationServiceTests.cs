// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.ContentManagement.Brokers.Authorizations;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Foundations.Authorizations;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Foundations.Authorizations;

public sealed partial class PageAuthorizationServiceTests
{
    [Fact]
    public async Task ShouldPrefetchCacheUsingUserDefaultCultureWhenRequestCultureIsEmptyAsync()
    {
        // Given
        PageRenderCache expectedCache = new()
        {
            PageId = 17,
            Culture = "en-gb",
            Theme = "default"
        };

        PageAuthorizationData authorizationData = new()
        {
            CacheLookupCompleted = true,
            Result = new PageAuthorizationResult
            {
                AppId = 3,
                PageId = 17,
                DefaultCulture = "en",
                UserId = "Paul",
                UserDefaultCultureId = "en-GB",
                CacheCandidates = [expectedCache]
            }
        };

        Mock<IPageAuthorizationBroker> authorizationBroker = new();
        Mock<IPageAuthorizationCacheBroker> cacheBroker = new();

        authorizationBroker
            .Setup(expression: broker => broker.GetCurrentUserId())
            .Returns(value: "Paul");

        authorizationBroker
            .Setup(expression: broker => broker.GetAuthorizedPageAsync(
                domain: "localhost",
                path: "Admin/AppManagement",
                culture: string.Empty,
                theme: string.Empty))
            .ReturnsAsync(value: authorizationData);

        PageAuthorizationService service = new(
            pageAuthorizationBroker: authorizationBroker.Object,
            pageAuthorizationCacheBroker: cacheBroker.Object);

        HttpPageRenderContext context = new()
        {
            Domain = "localhost",
            Path = "Admin/AppManagement",
            Culture = string.Empty,
            Theme = string.Empty
        };

        // When
        HttpPageRenderContext result = await service
            .AuthorizeHttpPageRenderContextAsync(
                httpPageRenderContext: context);

        // Then
        Assert.Same(expected: expectedCache, actual: result.PrefetchedPageRenderCache);
        Assert.True(condition: result.PageRenderCacheLookupCompleted);
        authorizationBroker.VerifyAll();
    }
}