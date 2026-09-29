// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class TemplateEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseTemplateUpdateEventAsync()
    {
        // Given
        Template entity = CreateRandomTemplate();

        templateEventServiceMock
            .Setup(expression: x => x.RaiseTemplateUpdateEventAsync(
                entity: entity,
                userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseTemplateUpdateEventAsync(
            template: entity,
            userId: CurrentUserId);

        // Then
        templateEventServiceMock.Verify(expression: x => x.RaiseTemplateUpdateEventAsync(
            entity: entity,
            userId: CurrentUserId), times: Times.Once);

        templateEventServiceMock.VerifyNoOtherCalls();
    }

}