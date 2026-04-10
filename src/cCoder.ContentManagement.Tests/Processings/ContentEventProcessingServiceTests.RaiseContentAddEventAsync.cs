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

public partial class ContentEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseContentAddEventAsync()
    {
        // Given
        Content entity = CreateRandomContent();
        contentEventServiceMock
            .Setup(x => x.RaiseContentAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseContentAddEventAsync(entity);

        // Then
        contentEventServiceMock.Verify(x => x.RaiseContentAddEventAsync(entity), Times.Once);
        contentEventServiceMock.VerifyNoOtherCalls();
    }

}
















