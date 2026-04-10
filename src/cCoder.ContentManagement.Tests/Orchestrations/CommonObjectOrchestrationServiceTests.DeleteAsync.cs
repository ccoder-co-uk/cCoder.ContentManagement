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

public partial class CommonObjectOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        int id = 1;
        CommonObject entity = CreateRandomCommonObject();
        commonObjectProcessingServiceMock.Setup(x => x.Get(id)).Returns(entity);
        commonObjectProcessingServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);

        commonObjectEventProcessingServiceMock
            .Setup(x => x.RaiseCommonObjectDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(id);

        // Then
        commonObjectProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        commonObjectProcessingServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        commonObjectEventProcessingServiceMock.Verify(x => x.RaiseCommonObjectDeleteEventAsync(entity), Times.Once);
    }

}




















