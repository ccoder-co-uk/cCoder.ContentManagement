// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageRenderCacheOrchestrationServiceTests
{
    [Fact]
    public void RefreshCommonObjectCache_WhenRequested_DelegatesToProcessingService()
    {
        // Given
        commonObjectCacheProcessingServiceMock.Setup(
            expression: service => service.Refresh());

        // When
        orchestrationService.RefreshCommonObjectCache();

        // Then
        commonObjectCacheProcessingServiceMock.VerifyAll();
    }
}