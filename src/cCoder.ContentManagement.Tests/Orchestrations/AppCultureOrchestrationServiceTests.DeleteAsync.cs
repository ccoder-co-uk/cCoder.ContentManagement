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


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class AppCultureOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        AppCulture appCulture = CreateRandomAppCulture();
        appCultureProcessingServiceMock.Setup(x => x.DeleteAsync(appCulture)).Returns(ValueTask.CompletedTask);

        appCultureEventProcessingServiceMock
            .Setup(x => x.RaiseAppCultureDeleteEventAsync(appCulture))
            .Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(appCulture);

        // Then
        appCultureProcessingServiceMock.Verify(x => x.DeleteAsync(appCulture), Times.Once);
        appCultureEventProcessingServiceMock.Verify(x => x.RaiseAppCultureDeleteEventAsync(appCulture), Times.Once);
    }

}




















