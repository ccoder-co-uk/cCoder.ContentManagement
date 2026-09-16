// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Caching;
using cCoder.ContentManagement.Rendering.Services.Processings;
using cCoder.ContentManagement.Services.Orchestrations.Caching;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.PageRendering;

public sealed partial class CommonObjectCacheRefreshPathTests
{
    [Fact]
    public void Refresh_WhenRequested_LoadsAndStoresOneSnapshot()
    {
        // Given
        CommonObjectCacheSnapshot snapshot = new();
        Mock<ICommonObjectCacheProcessingService> cacheProcessing = new();
        Mock<ICommonObjectLatestCacheProcessingService> sourceProcessing = new();

        sourceProcessing
            .Setup(expression: service =>
                service.LoadCommonObjectCacheSnapshot())
            .Returns(value: snapshot);

        cacheProcessing.Setup(expression: service =>
            service.SetCommonObjectCacheSnapshot(
                commonObjectCacheSnapshot: snapshot));

        CommonObjectCacheOrchestrationService service = new(
            cacheProcessingService: cacheProcessing.Object,
            latestCacheProcessingService: sourceProcessing.Object);

        // When
        service.Refresh();

        // Then
        sourceProcessing.VerifyAll();
        cacheProcessing.VerifyAll();
    }
}