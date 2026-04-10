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

public partial class CultureProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<Culture> entities = new[] { CreateRandomCulture() }.AsQueryable();
        cultureServiceMock.Setup(x => x.GetAll()).Returns(entities);

        // When
        IQueryable<Culture> result = cultureProcessingService.GetAll();

        // Then
        result.Should().BeSameAs(entities);
        cultureServiceMock.Verify(x => x.GetAll(), Times.Once);
        cultureServiceMock.VerifyNoOtherCalls();
    }

}
















