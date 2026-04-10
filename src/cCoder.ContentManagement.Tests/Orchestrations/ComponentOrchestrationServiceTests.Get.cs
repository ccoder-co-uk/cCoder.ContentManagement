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

public partial class ComponentOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        int id = 1;
        Component entity = CreateRandomComponent();
        componentProcessingServiceMock.Setup(x => x.Get(id)).Returns(entity);

        // When
        Component result = orchestrationService.Get(id);

        // Then
        result.Should().BeEquivalentTo(entity);
        componentProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        componentProcessingServiceMock.VerifyNoOtherCalls();
        componentEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}





















