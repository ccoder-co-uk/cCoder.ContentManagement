// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using Moq;

namespace cCoder.ContentManagement.Tests.Orchestrations;

public partial class CachedPageRenderOrchestrationServiceTests
{
    private readonly Mock<IPageRenderCacheQueryProcessingService>
        pageRenderCacheProcessingServiceMock = new();

    private readonly Mock<IPageRenderCacheMissEventProcessingService>
        eventProcessingServiceMock = new();

    private readonly CachedPageRenderOrchestrationService service;

    public CachedPageRenderOrchestrationServiceTests()
    {
        service = new CachedPageRenderOrchestrationService(
            pageRenderCacheProcessingService:
                pageRenderCacheProcessingServiceMock.Object,
            eventProcessingService: eventProcessingServiceMock.Object);
    }
}