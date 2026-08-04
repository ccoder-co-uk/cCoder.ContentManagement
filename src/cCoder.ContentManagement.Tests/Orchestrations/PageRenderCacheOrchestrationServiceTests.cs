// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;
using Moq;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageRenderCacheOrchestrationServiceTests
{
    private readonly Mock<IPageRenderCacheQueryProcessingService> queryProcessingServiceMock;
    private readonly Mock<IPageRenderCacheProcessingService> processingServiceMock;
    private readonly PageRenderCacheOrchestrationService orchestrationService;

    public PageRenderCacheOrchestrationServiceTests()
    {
        queryProcessingServiceMock = new Mock<IPageRenderCacheQueryProcessingService>(
            behavior: MockBehavior.Strict);

        processingServiceMock = new Mock<IPageRenderCacheProcessingService>(
            behavior: MockBehavior.Strict);

        IPageRenderCacheQueryProcessingService queryProcessingService =
            queryProcessingServiceMock.Object;

        IPageRenderCacheProcessingService processingService =
            processingServiceMock.Object;

        orchestrationService = new PageRenderCacheOrchestrationService(
            queryProcessingService: queryProcessingService,
            processingService: processingService);
    }

    private static PageRenderCache CreatePageRenderCache(
        int appId = 1,
        int pageId = 2) =>
        new()
        {
            Id = $"{appId}_{pageId}_en-gb_default",
            AppId = appId,
            PageId = pageId,
            Culture = "en-gb",
            Theme = "default",
            Path = "cached",
            Header = "header",
            Body = "body",
            SourceFingerprint = new string(c: 'A', count: 64),
            RenderedOn = DateTimeOffset.UtcNow
        };
}