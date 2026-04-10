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

public partial class LayoutProcessingServiceTests
{
    [Fact]
    public async Task ShouldUseFoundationDeleteAsyncPerItemWhenDeleteAllAsync()
    {
        // Given
        Layout entity = CreateRandomLayout();
        var id = entity.Id;
        layoutServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);

        // When
        await layoutProcessingService.DeleteAllAsync(new[] { entity });

        // Then
        layoutServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        layoutServiceMock.VerifyNoOtherCalls();
    }

}
















