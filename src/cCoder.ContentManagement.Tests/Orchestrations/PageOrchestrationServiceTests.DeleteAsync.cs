// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        int id = 1;
        Page entity = CreateRandomPage();

        pageProcessingServiceMock.Setup(expression: x => x.GetPage(pageId: id))
            .Returns(value: entity);

        pageProcessingServiceMock.Setup(expression: x => x.DeleteAsync(pageId: id))
            .Returns(value: ValueTask.CompletedTask);

        pageEventProcessingServiceMock
            .Setup(expression: x => x.RaisePageDeleteEventAsync(entity: entity, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(pageId: id);

        // Then
        pageProcessingServiceMock.Verify(expression: x => x.GetPage(pageId: id), times: Times.Once);
        pageProcessingServiceMock.Verify(expression: x => x.DeleteAsync(pageId: id), times: Times.Once);
        pageEventProcessingServiceMock.Verify(expression: x => x.RaisePageDeleteEventAsync(entity: entity, userId: CurrentUserId), times: Times.Once);
    }

}