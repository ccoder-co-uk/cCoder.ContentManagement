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


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ScriptEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseScriptUpdateEventAsync()
    {
        // Given
        Script entity = CreateRandomScript();

        scriptEventServiceMock
            .Setup(expression: x => x.RaiseScriptUpdateEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseScriptUpdateEventAsync(entity: entity);

        // Then
        scriptEventServiceMock.Verify(expression: x => x.RaiseScriptUpdateEventAsync(entity: entity), times: Times.Once);
        scriptEventServiceMock.VerifyNoOtherCalls();
    }

}