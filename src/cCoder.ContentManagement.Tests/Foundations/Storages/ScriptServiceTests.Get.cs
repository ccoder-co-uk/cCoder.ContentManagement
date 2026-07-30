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


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ScriptServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        Script script = CreateRandomScript(id: 7);

        scriptBrokerMock.Setup(expression: x => x.GetAllScripts())
            .Returns(value: new[] { script }.AsQueryable());

        // When
        Script result = scriptService.GetScript(scriptId: 7);

        // Then

        result.Should()
            .BeEquivalentTo(expectation: script);

        scriptBrokerMock.Verify(expression: x => x.GetAllScripts(), times: Times.Once);
        scriptBrokerMock.VerifyNoOtherCalls();
        authorizationManagerMock.VerifyNoOtherCalls();
    }

}