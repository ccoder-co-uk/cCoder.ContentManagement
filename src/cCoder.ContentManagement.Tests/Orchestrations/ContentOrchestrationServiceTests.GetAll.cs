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
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<Content> entities = new[] { CreateRandomContent() }.AsQueryable();
        contentProcessingServiceMock.Setup(x => x.GetAllContent(true)).Returns(entities);

        // When
        var result = orchestrationService.GetAllContent(true).ToArray();

        // Then
        result.Select(item => item.Id).Should().Equal(entities.Select(item => item.Id));
        contentProcessingServiceMock.Verify(x => x.GetAllContent(true), Times.Once);
        contentProcessingServiceMock.VerifyNoOtherCalls();
        contentEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}





















