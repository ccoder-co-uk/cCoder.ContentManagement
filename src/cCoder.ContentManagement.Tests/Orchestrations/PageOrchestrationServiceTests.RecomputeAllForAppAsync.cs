// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenRecomputeAllForAppAsync()
    {
        // Given
        pageProcessingServiceMock
            .Setup(expression: x => x.RecomputeAllForAppAsync(appId: 1))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.RecomputeAllForAppAsync(appId: 1);

        // Then
        pageProcessingServiceMock.Verify(expression: x => x.RecomputeAllForAppAsync(appId: 1), times: Times.Once);
        pageProcessingServiceMock.VerifyNoOtherCalls();
        pageEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}