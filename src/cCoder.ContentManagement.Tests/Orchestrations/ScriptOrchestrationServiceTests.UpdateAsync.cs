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
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class ScriptOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        Script entity = CreateRandomScript();

        scriptProcessingServiceMock.Setup(expression: x => x.UpdateScriptAsync(updatedScript: entity))
            .ReturnsAsync(value: entity);

        scriptEventProcessingServiceMock
            .Setup(expression: x => x.RaiseScriptUpdateEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Script result = await orchestrationService.UpdateScriptAsync(updatedScript: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        scriptProcessingServiceMock.Verify(expression: x => x.UpdateScriptAsync(updatedScript: entity), times: Times.Once);
        scriptEventProcessingServiceMock.Verify(expression: x => x.RaiseScriptUpdateEventAsync(entity: entity), times: Times.Once);
    }

}