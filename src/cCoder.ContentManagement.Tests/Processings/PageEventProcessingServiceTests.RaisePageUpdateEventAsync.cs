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

public partial class PageEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaisePageUpdateEventAsync()
    {
        // Given
        Page entity = CreateRandomPage();
        pageEventServiceMock
            .Setup(x => x.RaisePageUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaisePageUpdateEventAsync(entity);

        // Then
        pageEventServiceMock.Verify(x => x.RaisePageUpdateEventAsync(entity), Times.Once);
        pageEventServiceMock.VerifyNoOtherCalls();
    }

}
















