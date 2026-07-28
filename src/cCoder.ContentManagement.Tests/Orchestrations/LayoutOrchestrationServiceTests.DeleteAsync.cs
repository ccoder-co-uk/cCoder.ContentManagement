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

public partial class LayoutOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        int id = 1;
        Layout entity = CreateRandomLayout();

        layoutProcessingServiceMock.Setup(expression: x => x.GetLayout(layoutId: id))
            .Returns(value: entity);

        layoutProcessingServiceMock.Setup(expression: x => x.DeleteAsync(layoutId: id))
            .Returns(value: ValueTask.CompletedTask);

        layoutEventProcessingServiceMock
            .Setup(expression: x => x.RaiseLayoutDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(layoutId: id);

        // Then
        layoutProcessingServiceMock.Verify(expression: x => x.GetLayout(layoutId: id), times: Times.Once);
        layoutProcessingServiceMock.Verify(expression: x => x.DeleteAsync(layoutId: id), times: Times.Once);
        layoutEventProcessingServiceMock.Verify(expression: x => x.RaiseLayoutDeleteEventAsync(entity: entity), times: Times.Once);
    }

}