// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class CultureOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        string id = Guid.NewGuid()
            .ToString();

        Culture entity = CreateRandomCulture();

        cultureProcessingServiceMock.Setup(expression: x => x.GetCulture(cultureId: id))
            .Returns(value: entity);

        cultureProcessingServiceMock.Setup(expression: x => x.DeleteAsync(cultureId: id))
            .Returns(value: ValueTask.CompletedTask);

        cultureEventProcessingServiceMock
            .Setup(expression: x => x.RaiseCultureDeleteEventAsync(entity: entity, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(cultureId: id);

        // Then
        cultureProcessingServiceMock.Verify(expression: x => x.GetCulture(cultureId: id), times: Times.Once);
        cultureProcessingServiceMock.Verify(expression: x => x.DeleteAsync(cultureId: id), times: Times.Once);
        cultureEventProcessingServiceMock.Verify(expression: x => x.RaiseCultureDeleteEventAsync(entity: entity, userId: CurrentUserId), times: Times.Once);
    }

}