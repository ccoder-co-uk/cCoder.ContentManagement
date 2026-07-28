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
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        int id = 1;
        Template entity = CreateRandomTemplate();

        templateProcessingServiceMock.Setup(expression: x => x.GetTemplate(templateId: id))
            .Returns(value: entity);

        templateProcessingServiceMock.Setup(expression: x => x.DeleteAsync(templateId: id))
            .Returns(value: ValueTask.CompletedTask);

        templateEventProcessingServiceMock
            .Setup(expression: x => x.RaiseTemplateDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(templateId: id);

        // Then
        templateProcessingServiceMock.Verify(expression: x => x.GetTemplate(templateId: id), times: Times.Once);
        templateProcessingServiceMock.Verify(expression: x => x.DeleteAsync(templateId: id), times: Times.Once);
        templateEventProcessingServiceMock.Verify(expression: x => x.RaiseTemplateDeleteEventAsync(entity: entity), times: Times.Once);
    }

}