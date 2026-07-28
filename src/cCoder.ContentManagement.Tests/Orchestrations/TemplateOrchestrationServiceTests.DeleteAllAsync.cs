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

public partial class TemplateOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        Template[] entities = [CreateRandomTemplate()];

        templateProcessingServiceMock.Setup(expression: x => x.GetTemplate(templateId: entities[0].Id))
            .Returns(value: entities[0]);

        templateEventProcessingServiceMock.Setup(expression: x => x.RaiseTemplateDeleteEventAsync(entity: entities[0]))
            .Returns(value: ValueTask.CompletedTask);

        templateProcessingServiceMock.Setup(expression: x => x.DeleteAsync(templateId: entities[0].Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllTemplateAsync(deletedTemplate: entities);

        // Then
        templateProcessingServiceMock.Verify(expression: x => x.GetTemplate(templateId: entities[0].Id), times: Times.Once);
        templateEventProcessingServiceMock.Verify(expression: x => x.RaiseTemplateDeleteEventAsync(entity: entities[0]), times: Times.Once);
        templateProcessingServiceMock.Verify(expression: x => x.DeleteAsync(templateId: entities[0].Id), times: Times.Once);
    }

}