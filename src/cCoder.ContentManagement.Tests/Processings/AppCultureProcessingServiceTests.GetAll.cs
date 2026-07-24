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

public partial class AppCultureProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<AppCulture> entities = new[] { CreateRandomAppCulture() }.AsQueryable();
        appCultureServiceMock.Setup(x => x.GetAllAppCulture()).Returns(entities);

        // When
        IQueryable<AppCulture> result = appCultureProcessingService.GetAllAppCulture();

        // Then
        result.Should().BeSameAs(entities);
        appCultureServiceMock.Verify(x => x.GetAllAppCulture(), Times.Once);
        appCultureServiceMock.VerifyNoOtherCalls();
    }

}
















