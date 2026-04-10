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

public partial class CultureOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        string id = Guid.NewGuid().ToString();
        Culture entity = CreateRandomCulture();
        cultureProcessingServiceMock.Setup(x => x.Get(id)).Returns(entity);
        cultureProcessingServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);

        cultureEventProcessingServiceMock
            .Setup(x => x.RaiseCultureDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(id);

        // Then
        cultureProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        cultureProcessingServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        cultureEventProcessingServiceMock.Verify(x => x.RaiseCultureDeleteEventAsync(entity), Times.Once);
    }

}





















