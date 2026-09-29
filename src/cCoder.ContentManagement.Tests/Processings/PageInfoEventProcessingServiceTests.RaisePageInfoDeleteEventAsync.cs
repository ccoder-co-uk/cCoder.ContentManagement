// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageInfoEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaisePageInfoDeleteEventAsync()
    {
        // Given
        PageInfo entity = CreateRandomPageInfo();

        pageInfoEventServiceMock
            .Setup(expression: x => x.RaisePageInfoDeleteEventAsync(
                entity: entity,
                userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaisePageInfoDeleteEventAsync(
            pageInfo: entity,
            userId: CurrentUserId);

        // Then
        pageInfoEventServiceMock.Verify(expression: x => x.RaisePageInfoDeleteEventAsync(
            entity: entity,
            userId: CurrentUserId), times: Times.Once);

        pageInfoEventServiceMock.VerifyNoOtherCalls();
    }

}