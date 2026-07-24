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

public partial class PageInfoProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenAddAsync()
    {
        // Given
        PageInfo entity = CreateRandomPageInfo();
        pageInfoServiceMock.Setup(x => x.AddPageInfoAsync(entity)).ReturnsAsync(entity);

        // When
        PageInfo result = await pageInfoProcessingService.AddPageInfoAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        pageInfoServiceMock.Verify(x => x.AddPageInfoAsync(entity), Times.Once);
        pageInfoServiceMock.VerifyNoOtherCalls();
    }

}


















