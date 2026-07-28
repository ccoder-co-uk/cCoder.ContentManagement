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
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ScriptProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenUpdateAsync()
    {
        // Given
        Script entity = CreateRandomScript();

        scriptServiceMock.Setup(expression: x => x.UpdateScriptAsync(updatedScript: entity))
            .ReturnsAsync(value: entity);

        // When
        Script result = await scriptProcessingService.UpdateScriptAsync(updatedScript: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        scriptServiceMock.Verify(expression: x => x.UpdateScriptAsync(updatedScript: entity), times: Times.Once);
        scriptServiceMock.VerifyNoOtherCalls();
    }

}