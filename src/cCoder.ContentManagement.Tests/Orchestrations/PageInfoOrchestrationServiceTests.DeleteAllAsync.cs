// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageInfoOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        PageInfo[] entities = [CreateRandomPageInfo()];

        pageInfoProcessingServiceMock.Setup(expression: x => x.GetPageInfo(pageInfoId: entities[0].Id))
            .Returns(value: entities[0]);

        pageInfoEventProcessingServiceMock.Setup(expression: x => x.RaisePageInfoDeleteEventAsync(entity: entities[0], userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        pageInfoProcessingServiceMock.Setup(expression: x => x.DeleteAsync(pageInfoId: entities[0].Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllPageInfoAsync(deletedPageInfo: entities);

        // Then
        pageInfoProcessingServiceMock.Verify(expression: x => x.GetPageInfo(pageInfoId: entities[0].Id), times: Times.Once);
        pageInfoEventProcessingServiceMock.Verify(expression: x => x.RaisePageInfoDeleteEventAsync(entity: entities[0], userId: CurrentUserId), times: Times.Once);
        pageInfoProcessingServiceMock.Verify(expression: x => x.DeleteAsync(pageInfoId: entities[0].Id), times: Times.Once);
    }

}