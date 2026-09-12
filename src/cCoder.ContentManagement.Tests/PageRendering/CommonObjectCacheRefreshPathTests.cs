// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures.Caching;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Rendering.Services.Foundations;
using cCoder.ContentManagement.Rendering.Services.Processings;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.PageRendering;

public sealed partial class CommonObjectCacheRefreshPathTests
{
    [Fact]
    public void CommonObjectReaderBroker_WhenRefreshRequested_RefreshesDependency()
    {
        // Given
        Mock<ICommonObjectCache> commonObjectCache = new();
        commonObjectCache.Setup(expression: cache => cache.Refresh());
        CommonObjectReaderBroker broker = new(commonObjectCache: commonObjectCache.Object);

        // When
        broker.Refresh();

        // Then
        commonObjectCache.VerifyAll();
    }

    [Fact]
    public void CommonObjectCacheService_WhenRefreshRequested_RefreshesBroker()
    {
        // Given
        Mock<ICommonObjectReaderBroker> broker = new();
        broker.Setup(expression: cache => cache.Refresh());
        CommonObjectCacheService service = new(broker: broker.Object);

        // When
        service.Refresh();

        // Then
        broker.VerifyAll();
    }

    [Fact]
    public void CommonObjectCacheProcessingService_WhenRefreshRequested_RefreshesFoundationService()
    {
        // Given
        Mock<ICommonObjectCacheService> service = new();
        service.Setup(expression: cache => cache.Refresh());

        CommonObjectCacheProcessingService processingService = new(
            commonObjectCacheService: service.Object);

        // When
        processingService.Refresh();

        // Then
        service.VerifyAll();
    }
}