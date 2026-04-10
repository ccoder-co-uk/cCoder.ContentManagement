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

public partial class AppEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseAppDeleteEventAsync()
    {
        // Given
        App app = CreateRandomApp();
        appEventServiceMock
            .Setup(x => x.RaiseAppDeleteEventAsync(app))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseAppDeleteEventAsync(app);

        // Then
        appEventServiceMock.Verify(x => x.RaiseAppDeleteEventAsync(app), Times.Once);
        appEventServiceMock.VerifyNoOtherCalls();
    }

}
















