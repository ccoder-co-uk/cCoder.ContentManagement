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

public partial class ResourceProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        Resource[] resources = [CreateRandomResource()];
        IQueryable<Resource> queryableResources = resources.AsQueryable();
        resourceServiceMock.Setup(x => x.GetAll()).Returns(queryableResources);

        // When
        IQueryable<Resource> result = resourceProcessingService.GetAll();

        // Then
        result.Should().BeSameAs(queryableResources);
        resourceServiceMock.Verify(x => x.GetAll(), Times.Once);
        resourceServiceMock.VerifyNoOtherCalls();
    }

}















