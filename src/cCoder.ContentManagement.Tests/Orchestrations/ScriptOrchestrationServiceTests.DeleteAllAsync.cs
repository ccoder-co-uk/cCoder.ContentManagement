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

public partial class ScriptOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        Script[] entities = [CreateRandomScript()];

        scriptProcessingServiceMock.Setup(expression: x => x.GetScript(scriptId: entities[0].Id))
            .Returns(value: entities[0]);

        scriptEventProcessingServiceMock.Setup(expression: x => x.RaiseScriptDeleteEventAsync(entity: entities[0]))
            .Returns(value: ValueTask.CompletedTask);

        scriptProcessingServiceMock.Setup(expression: x => x.DeleteAsync(scriptId: entities[0].Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllScriptAsync(deletedScript: entities);

        // Then
        scriptProcessingServiceMock.Verify(expression: x => x.GetScript(scriptId: entities[0].Id), times: Times.Once);
        scriptEventProcessingServiceMock.Verify(expression: x => x.RaiseScriptDeleteEventAsync(entity: entities[0]), times: Times.Once);
        scriptProcessingServiceMock.Verify(expression: x => x.DeleteAsync(scriptId: entities[0].Id), times: Times.Once);
    }

}