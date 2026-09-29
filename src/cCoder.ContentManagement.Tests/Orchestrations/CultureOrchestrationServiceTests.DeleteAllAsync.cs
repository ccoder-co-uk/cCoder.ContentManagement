// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class CultureOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        Culture[] entities = [CreateRandomCulture()];

        cultureProcessingServiceMock.Setup(expression: x => x.DeleteAllCultureAsync(deletedCulture: entities))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllCultureAsync(deletedCulture: entities);

        // Then
        cultureProcessingServiceMock.Verify(expression: x => x.DeleteAllCultureAsync(deletedCulture: entities), times: Times.Once);

        cultureProcessingServiceMock.Verify(
            expression: x => x.GetOwningAppId(
                cultureId: entities[0].Id),
            times: Times.Once);

        cultureProcessingServiceMock.VerifyNoOtherCalls();
        cultureEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}