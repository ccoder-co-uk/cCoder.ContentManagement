// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageInfoOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        PageInfo entity = CreateRandomPageInfo();

        pageInfoProcessingServiceMock.Setup(expression: x => x.UpdatePageInfoAsync(updatedPageInfo: entity))
            .ReturnsAsync(value: entity);

        pageInfoEventProcessingServiceMock
            .Setup(expression: x => x.RaisePageInfoUpdateEventAsync(entity: entity, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        PageInfo result = await orchestrationService.UpdatePageInfoAsync(updatedPageInfo: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        pageInfoProcessingServiceMock.Verify(expression: x => x.UpdatePageInfoAsync(updatedPageInfo: entity), times: Times.Once);
        pageInfoEventProcessingServiceMock.Verify(expression: x => x.RaisePageInfoUpdateEventAsync(entity: entity, userId: CurrentUserId), times: Times.Once);
    }

}