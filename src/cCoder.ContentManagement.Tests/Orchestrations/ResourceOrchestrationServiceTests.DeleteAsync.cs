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
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        int id = 1;
        Resource entity = CreateRandomResource();

        resourceProcessingServiceMock.Setup(expression: x => x.GetResource(resourceId: id))
            .Returns(value: entity);

        resourceProcessingServiceMock.Setup(expression: x => x.DeleteAsync(resourceId: id))
            .Returns(value: ValueTask.CompletedTask);

        resourceEventProcessingServiceMock
            .Setup(expression: x => x.RaiseResourceDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(resourceId: id);

        // Then
        resourceProcessingServiceMock.Verify(expression: x => x.GetResource(resourceId: id), times: Times.Once);
        resourceProcessingServiceMock.Verify(expression: x => x.DeleteAsync(resourceId: id), times: Times.Once);
        resourceEventProcessingServiceMock.Verify(expression: x => x.RaiseResourceDeleteEventAsync(entity: entity), times: Times.Once);
    }

}