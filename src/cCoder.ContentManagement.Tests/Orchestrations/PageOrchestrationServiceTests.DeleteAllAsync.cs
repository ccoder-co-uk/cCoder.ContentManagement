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

public partial class PageOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        Page[] entities = [CreateRandomPage()];

        pageProcessingServiceMock.Setup(expression: x => x.GetPage(pageId: entities[0].Id))
            .Returns(value: entities[0]);

        pageEventProcessingServiceMock.Setup(expression: x => x.RaisePageDeleteEventAsync(entity: entities[0]))
            .Returns(value: ValueTask.CompletedTask);

        pageProcessingServiceMock.Setup(expression: x => x.DeleteAsync(pageId: entities[0].Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllPageAsync(deletedPage: entities);

        // Then
        pageProcessingServiceMock.Verify(expression: x => x.GetPage(pageId: entities[0].Id), times: Times.Once);
        pageEventProcessingServiceMock.Verify(expression: x => x.RaisePageDeleteEventAsync(entity: entities[0]), times: Times.Once);
        pageProcessingServiceMock.Verify(expression: x => x.DeleteAsync(pageId: entities[0].Id), times: Times.Once);
    }

}