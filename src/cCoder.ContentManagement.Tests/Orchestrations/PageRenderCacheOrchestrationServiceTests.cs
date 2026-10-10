// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Brokers.Authorizations;
using Moq;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageRenderCacheOrchestrationServiceTests
{
    private readonly Mock<IPageRenderCacheProcessingService> processingServiceMock;
    private readonly Mock<IAuthorizationProcessingService> authorizationProcessingServiceMock;
    private readonly Mock<IPageAuthorizationCacheBroker> pageAuthorizationCacheBrokerMock;
    private readonly PageRenderCacheOrchestrationService orchestrationService;

    public PageRenderCacheOrchestrationServiceTests()
    {
        processingServiceMock = new Mock<IPageRenderCacheProcessingService>(
            behavior: MockBehavior.Strict);

        authorizationProcessingServiceMock = new(behavior: MockBehavior.Strict);
        pageAuthorizationCacheBrokerMock = new(behavior: MockBehavior.Loose);

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                context: It.IsAny<AuthorizationContext>()));

        IPageRenderCacheProcessingService processingService =
            processingServiceMock.Object;

        orchestrationService = new PageRenderCacheOrchestrationService(
            processingService: processingService,
            authorizationProcessingService: authorizationProcessingServiceMock.Object,
            pageAuthorizationCacheBroker: pageAuthorizationCacheBrokerMock.Object);
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