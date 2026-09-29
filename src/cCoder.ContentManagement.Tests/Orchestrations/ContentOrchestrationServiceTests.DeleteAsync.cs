// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class ContentOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        int id = 1;
        Content entity = CreateRandomContent();

        contentProcessingServiceMock.Setup(expression: x => x.GetContent(contentId: id))
            .Returns(value: entity);

        contentProcessingServiceMock.Setup(expression: x => x.DeleteAsync(contentId: id))
            .Returns(value: ValueTask.CompletedTask);

        contentEventProcessingServiceMock
            .Setup(expression: x => x.RaiseContentDeleteEventAsync(entity: entity, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(contentId: id);

        // Then
        contentProcessingServiceMock.Verify(expression: x => x.GetContent(contentId: id), times: Times.Once);
        contentProcessingServiceMock.Verify(expression: x => x.DeleteAsync(contentId: id), times: Times.Once);
        contentEventProcessingServiceMock.Verify(expression: x => x.RaiseContentDeleteEventAsync(entity: entity, userId: CurrentUserId), times: Times.Once);
    }

}