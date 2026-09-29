// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class CultureOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        Culture entity = CreateRandomCulture();

        cultureProcessingServiceMock.Setup(expression: x => x.UpdateCultureAsync(updatedCulture: entity))
            .ReturnsAsync(value: entity);

        cultureEventProcessingServiceMock
            .Setup(expression: x => x.RaiseCultureUpdateEventAsync(entity: entity, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Culture result = await orchestrationService.UpdateCultureAsync(updatedCulture: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        cultureProcessingServiceMock.Verify(expression: x => x.UpdateCultureAsync(updatedCulture: entity), times: Times.Once);
        cultureEventProcessingServiceMock.Verify(expression: x => x.RaiseCultureUpdateEventAsync(entity: entity, userId: CurrentUserId), times: Times.Once);
    }

}