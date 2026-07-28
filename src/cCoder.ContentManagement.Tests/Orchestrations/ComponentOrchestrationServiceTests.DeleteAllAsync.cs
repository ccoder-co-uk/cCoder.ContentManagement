// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.ContentManagementConfiguration;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class ComponentOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        Component[] entities = [CreateRandomComponent()];

        componentProcessingServiceMock.Setup(expression: x => x.GetComponent(componentId: entities[0].Id))
            .Returns(value: entities[0]);

        componentEventProcessingServiceMock.Setup(expression: x => x.RaiseComponentDeleteEventAsync(entity: entities[0]))
            .Returns(value: ValueTask.CompletedTask);

        componentProcessingServiceMock.Setup(expression: x => x.DeleteAsync(componentId: entities[0].Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllComponentAsync(deletedComponent: entities);

        // Then
        componentProcessingServiceMock.Verify(expression: x => x.GetComponent(componentId: entities[0].Id), times: Times.Once);
        componentEventProcessingServiceMock.Verify(expression: x => x.RaiseComponentDeleteEventAsync(entity: entities[0]), times: Times.Once);
        componentProcessingServiceMock.Verify(expression: x => x.DeleteAsync(componentId: entities[0].Id), times: Times.Once);
    }

}