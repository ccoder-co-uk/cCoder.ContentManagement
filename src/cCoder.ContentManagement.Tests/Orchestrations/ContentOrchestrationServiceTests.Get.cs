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

public partial class ContentOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        int id = 1;
        Content entity = CreateRandomContent();
        contentProcessingServiceMock.Setup(x => x.Get(id)).Returns(entity);

        // When
        Content result = orchestrationService.Get(id);

        // Then
        result.Should().BeEquivalentTo(entity);
        contentProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        contentProcessingServiceMock.VerifyNoOtherCalls();
        contentEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}





















