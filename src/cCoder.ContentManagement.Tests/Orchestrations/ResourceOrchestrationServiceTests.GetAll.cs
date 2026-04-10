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

public partial class ResourceOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<Resource> entities = new[] { CreateRandomResource() }.AsQueryable();
        resourceProcessingServiceMock.Setup(x => x.GetAll(true)).Returns(entities);

        // When
        var result = orchestrationService.GetAll(true).ToArray();

        // Then
        result.Select(item => item.Id).Should().Equal(entities.Select(item => item.Id));
        resourceProcessingServiceMock.Verify(x => x.GetAll(true), Times.Once);
        resourceProcessingServiceMock.VerifyNoOtherCalls();
        resourceEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}





















