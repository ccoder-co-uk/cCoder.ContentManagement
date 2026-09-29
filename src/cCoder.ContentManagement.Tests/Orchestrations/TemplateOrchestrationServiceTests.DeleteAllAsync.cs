// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class TemplateOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        Template[] entities = [CreateRandomTemplate()];

        templateProcessingServiceMock.Setup(expression: x => x.GetTemplate(templateId: entities[0].Id))
            .Returns(value: entities[0]);

        templateEventProcessingServiceMock.Setup(expression: x => x.RaiseTemplateDeleteEventAsync(entity: entities[0], userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        templateProcessingServiceMock.Setup(expression: x => x.DeleteAsync(templateId: entities[0].Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllTemplateAsync(deletedTemplate: entities);

        // Then
        templateProcessingServiceMock.Verify(expression: x => x.GetTemplate(templateId: entities[0].Id), times: Times.Once);
        templateEventProcessingServiceMock.Verify(expression: x => x.RaiseTemplateDeleteEventAsync(entity: entities[0], userId: CurrentUserId), times: Times.Once);
        templateProcessingServiceMock.Verify(expression: x => x.DeleteAsync(templateId: entities[0].Id), times: Times.Once);
    }

}