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

public partial class ResourceOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        Resource[] entities = [CreateRandomResource()];

        resourceProcessingServiceMock.Setup(expression: x => x.GetResource(resourceId: entities[0].Id))
            .Returns(value: entities[0]);

        resourceEventProcessingServiceMock.Setup(expression: x => x.RaiseResourceDeleteEventAsync(entity: entities[0]))
            .Returns(value: ValueTask.CompletedTask);

        resourceProcessingServiceMock.Setup(expression: x => x.DeleteAsync(resourceId: entities[0].Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllResourceAsync(deletedResource: entities);

        // Then
        resourceProcessingServiceMock.Verify(expression: x => x.GetResource(resourceId: entities[0].Id), times: Times.Once);
        resourceEventProcessingServiceMock.Verify(expression: x => x.RaiseResourceDeleteEventAsync(entity: entities[0]), times: Times.Once);
        resourceProcessingServiceMock.Verify(expression: x => x.DeleteAsync(resourceId: entities[0].Id), times: Times.Once);
    }

}