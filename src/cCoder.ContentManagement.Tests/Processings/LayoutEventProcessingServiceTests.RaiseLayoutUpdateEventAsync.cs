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

public partial class LayoutEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseLayoutUpdateEventAsync()
    {
        // Given
        Layout entity = CreateRandomLayout();
        layoutEventServiceMock
            .Setup(x => x.RaiseLayoutUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseLayoutUpdateEventAsync(entity);

        // Then
        layoutEventServiceMock.Verify(x => x.RaiseLayoutUpdateEventAsync(entity), Times.Once);
        layoutEventServiceMock.VerifyNoOtherCalls();
    }

}
















