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
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        int id = 1;
        Script entity = CreateRandomScript();
        scriptProcessingServiceMock.Setup(x => x.Get(id)).Returns(entity);

        // When
        Script result = orchestrationService.Get(id);

        // Then
        result.Should().BeEquivalentTo(entity);
        scriptProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        scriptProcessingServiceMock.VerifyNoOtherCalls();
        scriptEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}





















