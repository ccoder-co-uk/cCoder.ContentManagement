// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Rendering.Services.Foundations;
using cCoder.ContentManagement.Rendering.Services.Processings;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Foundations.Rendering;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Orchestrations.PageContexts;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Aggregations;

public sealed partial class RenderAggregationServiceTests
{
    [Fact]
    public async Task ShouldLazilyCacheOnlyRequestedVariantThenServeItAsync()
    {
        // Given
        List<PageRenderCache> caches = [];
        App app = new() { Id = 3, DefaultCultureId = "en", DefaultTheme = "Default" };
        Page page = new() { Id = 17, App = app };

        HttpPageRenderContext context = new()
        {
            AppId = app.Id,
            PageId = page.Id,
            Culture = "en-gb",
            Theme = "dark",
            Nonce = "request-nonce"
        };

        Mock<IPageRenderCacheService> cacheFoundation = new();

        cacheFoundation.Setup(expression: service =>
            service.GetAllPageRenderCaches())
            .Returns(valueFunction: () => caches.AsQueryable());

        cacheFoundation.Setup(expression: service =>
            service.GetPageRenderCache(
                pageRenderCacheId: "3_17_en-gb_dark"))
            .Returns(valueFunction: () => caches.SingleOrDefault());

        cacheFoundation.Setup(expression: service =>
            service.AddPageRenderCacheAsync(
                newPageRenderCache: It.IsAny<PageRenderCache>()))
            .Callback<PageRenderCache>(action: cache => caches.Add(item: cache))
            .ReturnsAsync(valueFunction: (PageRenderCache cache) => cache);

        PageRenderCacheProcessingService cacheProcessing = new(
            service: cacheFoundation.Object);

        PageRenderCacheQueryProcessingService cacheQuery = new(
            pageRenderCacheService: cacheFoundation.Object);

        CachedPageRenderOrchestrationService cached = new(
            queryProcessingService: cacheQuery,
            renderProcessingService: new CachedPageRenderProcessingService(
                cachedPageRenderService: new CachedPageRenderService(
                    regularExpressionBroker: new RegularExpressionBroker())),
            cacheProcessingService: cacheProcessing);

        Mock<IPageProcessingService> pageProcessing = new();
        Mock<IPageRenderProcessingService> renderProcessing = new();

        renderProcessing.Setup(expression: service =>
            service.SerializeRuntimeValue(value: It.IsAny<object>()))
            .Returns(value: "{}");

        pageProcessing.Setup(expression: service =>
            service.GetPageForRenderAsync(pageId: page.Id))
            .ReturnsAsync(value: page);

        renderProcessing.Setup(expression: service =>
            service.RenderPageRenderOperation(
                operation: It.IsAny<PageRenderOperation>()))
            .Returns(valueFunction: (PageRenderOperation operation) =>
            {
                operation.Page = new PageRenderResult
                {
                    AppId = app.Id,
                    PageId = page.Id,
                    Path = "Admin/AppManagement",
                    HeaderHtml = "<style nonce='[request[nonce]]'></style>",
                    BodyHtml = "<main>rendered</main>"
                };

                return operation;
            });

        renderProcessing.Setup(expression: service =>
            service.CompletePageRenderOperation(
                operation: It.IsAny<PageRenderOperation>()))
            .Returns(valueFunction: (PageRenderOperation operation) => operation);

        UncachedPageRenderOrchestrationService uncached = new(
            pageProcessingService: pageProcessing.Object,
            pageRenderProcessingService: renderProcessing.Object);

        Mock<IRenderEventOrchestrationService> renderEvent = new();

        renderEvent.Setup(expression: service =>
                service.RaiseHttpPageRenderOperationRenderRequestAsync(
                    httpPageRenderOperation: It.IsAny<HttpPageRenderOperation>()))
            .Callback<HttpPageRenderOperation>(action: operation =>
            {
                uncached.PrepareHttpPageRenderOperationAsync(
                        httpPageRenderOperation: operation)
                    .GetAwaiter()
                    .GetResult();

                uncached.CompleteHttpPageRenderOperationAsync(
                        httpPageRenderOperation: operation)
                    .GetAwaiter()
                    .GetResult();
            })
            .Returns(value: ValueTask.CompletedTask);

        Mock<IRenderDataOrchestrationService> json = CreateRenderDataOrchestrationServiceMock();

        json.Setup(expression: service =>
            service.SerializeRuntimeValue(value: It.IsAny<object>()))
            .Returns(valueFunction: (object value) =>
                new JsonBroker().Serialize(value: value));

        Mock<IPageContextOrchestrationService> pageContext = new();

        pageContext.Setup(expression: service =>
            service.ResolvePageRenderContextAsync())
            .Returns(value: ValueTask.FromResult(result: context));

        RenderAggregationService service = new(
            pageContextOrchestrationService: pageContext.Object,
            cachedPageRenderOrchestrationService: cached,
            renderEventOrchestrationService: renderEvent.Object,
            renderDataOrchestrationService: json.Object,
            templateRenderOrchestrationService:
                Mock.Of<ITemplateRenderOrchestrationService>(),
            componentRenderOrchestrationService:
                Mock.Of<IComponentRenderOrchestrationService>());

        // When
        RenderResult uncachedResult = await service.RenderPageRenderResultAsync();
        RenderResult cachedResult = await service.RenderPageRenderResultAsync();

        // Then
        PageRenderCache stored = Assert.Single(collection: caches);
        Assert.Equal(expected: "3_17_en-gb_dark", actual: stored.Id);

        Assert.DoesNotContain(
            collection: caches,
            filter: cache => cache.Culture == "fr" || cache.Theme == "light");

        Assert.Equal(
            expected: uncachedResult.PageResponse.Page.BodyHtml,
            actual: cachedResult.PageResponse.Page.BodyHtml);

        Assert.Equal(
            expected: uncachedResult.PageResponse.Page.HeaderHtml,
            actual: cachedResult.PageResponse.Page.HeaderHtml);

        pageProcessing.Verify(
            expression: processing => processing.GetPageForRenderAsync(
                pageId: page.Id),
            times: Times.Once);

        renderProcessing.Verify(
            expression: processing => processing.RenderPageRenderOperation(
                operation: It.IsAny<PageRenderOperation>()),
            times: Times.Once);
    }
}