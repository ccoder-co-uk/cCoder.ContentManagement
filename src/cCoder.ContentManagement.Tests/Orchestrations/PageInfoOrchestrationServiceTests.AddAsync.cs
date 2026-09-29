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
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        PageInfo entity = CreateRandomPageInfo();

        pageInfoProcessingServiceMock.Setup(expression: x => x.AddPageInfoAsync(newPageInfo: entity))
            .ReturnsAsync(value: entity);

        pageInfoEventProcessingServiceMock
            .Setup(expression: x => x.RaisePageInfoAddEventAsync(entity: entity, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        PageInfo result = await orchestrationService.AddPageInfoAsync(newPageInfo: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        pageInfoProcessingServiceMock.Verify(expression: x => x.AddPageInfoAsync(newPageInfo: entity), times: Times.Once);
        pageInfoEventProcessingServiceMock.Verify(expression: x => x.RaisePageInfoAddEventAsync(entity: entity, userId: CurrentUserId), times: Times.Once);
    }

}