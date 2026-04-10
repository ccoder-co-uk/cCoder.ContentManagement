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
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageInfoProcessingServiceTests
{
    [Fact]
    public async Task ShouldUseFoundationDeleteAsyncPerItemWhenDeleteAllAsync()
    {
        // Given
        PageInfo entity = CreateRandomPageInfo();
        var id = entity.Id;
        pageInfoServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);

        // When
        await pageInfoProcessingService.DeleteAllAsync(new[] { entity });

        // Then
        pageInfoServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        pageInfoServiceMock.VerifyNoOtherCalls();
    }

}


















