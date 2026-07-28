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
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        int id = 1;
        Component entity = CreateRandomComponent();

        componentProcessingServiceMock.Setup(expression: x => x.GetComponent(componentId: id))
            .Returns(value: entity);

        componentProcessingServiceMock.Setup(expression: x => x.DeleteAsync(componentId: id))
            .Returns(value: ValueTask.CompletedTask);

        componentEventProcessingServiceMock
            .Setup(expression: x => x.RaiseComponentDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(componentId: id);

        // Then
        componentProcessingServiceMock.Verify(expression: x => x.GetComponent(componentId: id), times: Times.Once);
        componentProcessingServiceMock.Verify(expression: x => x.DeleteAsync(componentId: id), times: Times.Once);
        componentEventProcessingServiceMock.Verify(expression: x => x.RaiseComponentDeleteEventAsync(entity: entity), times: Times.Once);
    }

}