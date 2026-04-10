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

public partial class PageInfoEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaisePageInfoDeleteEventAsync()
    {
        // Given
        PageInfo entity = CreateRandomPageInfo();
        pageInfoEventServiceMock
            .Setup(x => x.RaisePageInfoDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaisePageInfoDeleteEventAsync(entity);

        // Then
        pageInfoEventServiceMock.Verify(x => x.RaisePageInfoDeleteEventAsync(entity), Times.Once);
        pageInfoEventServiceMock.VerifyNoOtherCalls();
    }

}


















