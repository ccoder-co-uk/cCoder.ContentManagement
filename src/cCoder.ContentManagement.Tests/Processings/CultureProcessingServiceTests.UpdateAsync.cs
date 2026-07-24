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
    public async Task ShouldDelegateToFoundationServiceWhenUpdateAsync()
    {
        // Given
        Culture entity = CreateRandomCulture();
        cultureServiceMock.Setup(x => x.UpdateCultureAsync(entity)).ReturnsAsync(entity);

        // When
        Culture result = await cultureProcessingService.UpdateCultureAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        cultureServiceMock.Verify(x => x.UpdateCultureAsync(entity), Times.Once);
        cultureServiceMock.VerifyNoOtherCalls();
    }

}
















