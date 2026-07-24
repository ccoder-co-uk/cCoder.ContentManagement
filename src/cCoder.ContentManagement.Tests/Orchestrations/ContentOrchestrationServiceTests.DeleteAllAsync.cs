// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.Config;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class ContentOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        Content[] entities = [CreateRandomContent()];

        contentProcessingServiceMock.Setup(expression: x => x.GetContent(contentId: entities[0].Id))
            .Returns(value: entities[0]);

        contentEventProcessingServiceMock.Setup(expression: x => x.RaiseContentDeleteEventAsync(entity: entities[0]))
            .Returns(value: ValueTask.CompletedTask);

        contentProcessingServiceMock.Setup(expression: x => x.DeleteAsync(contentId: entities[0].Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllContentAsync(deletedContent: entities);

        // Then
        contentProcessingServiceMock.Verify(expression: x => x.GetContent(contentId: entities[0].Id), times: Times.Once);
        contentEventProcessingServiceMock.Verify(expression: x => x.RaiseContentDeleteEventAsync(entity: entities[0]), times: Times.Once);
        contentProcessingServiceMock.Verify(expression: x => x.DeleteAsync(contentId: entities[0].Id), times: Times.Once);
    }

}