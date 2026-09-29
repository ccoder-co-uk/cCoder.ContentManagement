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
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        Culture entity = CreateRandomCulture();

        cultureProcessingServiceMock.Setup(expression: x => x.AddCultureAsync(newCulture: entity))
            .ReturnsAsync(value: entity);

        cultureEventProcessingServiceMock
            .Setup(expression: x => x.RaiseCultureAddEventAsync(entity: entity, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Culture result = await orchestrationService.AddCultureAsync(newCulture: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        cultureProcessingServiceMock.Verify(expression: x => x.AddCultureAsync(newCulture: entity), times: Times.Once);
        cultureEventProcessingServiceMock.Verify(expression: x => x.RaiseCultureAddEventAsync(entity: entity, userId: CurrentUserId), times: Times.Once);
    }

}