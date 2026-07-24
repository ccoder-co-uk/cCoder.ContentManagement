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


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ScriptProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        Script entity = CreateRandomScript();
        var id = entity.Id;

        scriptServiceMock.Setup(expression: x => x.GetScript(scriptId: id))
            .Returns(value: entity);

        // When
        Script result = scriptProcessingService.GetScript(scriptId: id);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        scriptServiceMock.Verify(expression: x => x.GetScript(scriptId: id), times: Times.Once);
        scriptServiceMock.VerifyNoOtherCalls();
    }

}